extends PanelContainer

## Expandable alert list. Shows active alerts color-coded by priority.
## Click alert to pan camera to location. Dismiss per alert.

@onready var title_label: Label = $VBox/Title
@onready var close_button: Button = $VBox/Title/CloseButton
@onready var alert_list: VBoxContainer = $VBox/ScrollContainer/AlertList

var alert_item_scene: PackedScene
var _alert_system = null

signal alert_clicked(location: Vector3)
signal alert_dismissed(alert_id: String)

func _ready():
	# Get alert system
	var bootstrap = get_node_or_null("/root/Bootstrap")
	if bootstrap:
		# AlertSystem is a C# singleton, access via static
		pass
	
	# Connect close button
	if close_button:
		close_button.pressed.connect(func(): hide())
	
	# Start hidden
	hide()

func _on_alert_added(alert):
	if not alert:
		return
	
	var item = _create_alert_item(alert)
	alert_list.add_child(item)
	
	# Update title
	title_label.text = "⚠ ALERTS (%d)" % alert_list.get_child_count()
	
	show()

func _on_alert_dismissed(alert_id: String):
	for child in alert_list.get_children():
		if child.has_meta("alert_id") and child.get_meta("alert_id") == alert_id:
			child.queue_free()
			break
	
	var remaining = alert_list.get_child_count()
	title_label.text = "⚠ ALERTS (%d)" % remaining
	if remaining == 0:
		hide()

func _create_alert_item(alert) -> Control:
	var container = PanelContainer.new()
	container.set_meta("alert_id", alert.Id)
	
	# Color based on priority
	var priority = alert.get("Priority") if alert.has_method("get") else 0
	var bg_color: Color
	match int(priority):
		2: bg_color = Color(0.4, 0.1, 0.1, 0.8)  # Critical - red
		1: bg_color = Color(0.4, 0.3, 0.0, 0.8)   # Warning - yellow
		_: bg_color = Color(0.1, 0.3, 0.1, 0.8)   # Info - green
	
	var style = StyleBoxFlat.new()
	style.bg_color = bg_color
	style.corner_radius_top_left = 4
	style.corner_radius_top_right = 4
	style.corner_radius_bottom_left = 4
	style.corner_radius_bottom_right = 4
	style.content_margin_left = 8
	style.content_margin_right = 8
	style.content_margin_top = 4
	style.content_margin_bottom = 4
	container.add_theme_stylebox_override("panel", style)
	
	var hbox = HBoxContainer.new()
	container.add_child(hbox)
	
	# Message label
	var msg_label = Label.new()
	msg_label.text = alert.get("Message") if alert.has_method("get") else "Alert"
	msg_label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	hbox.add_child(msg_label)
	
	# Dismiss button
	var dismiss_btn = Button.new()
	dismiss_btn.text = "✕"
	dismiss_btn.custom_minimum_size = Vector2(24, 24)
	dismiss_btn.pressed.connect(func():
		var id = container.get_meta("alert_id")
		alert_dismissed.emit(id)
		container.queue_free()
	)
	hbox.add_child(dismiss_btn)
	
	# Click to pan to location
	container.gui_input.connect(func(event):
		if event is InputEventMouseButton and event.pressed and event.button_index == MOUSE_BUTTON_LEFT:
			var loc = alert.get("WorldLocation") if alert.has_method("get") else null
			if loc:
				alert_clicked.emit(loc)
	)
	
	return container
