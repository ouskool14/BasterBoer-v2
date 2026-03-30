extends PanelContainer

## Save/Load menu. Tab to switch between Save and Load modes.
## Lists existing saves with preview info. Allows new saves, overwrites, and deletes.

@onready var title_label: Label = $VBox/Title
@onready var save_tab: Button = $VBox/TabBar/SaveTab
@onready var load_tab: Button = $VBox/TabBar/LoadTab
@onready var slot_list: VBoxContainer = $VBox/ScrollContainer/SlotList
@onready var new_save_input: LineEdit = $VBox/NewSaveBar/SaveNameInput
@onready var new_save_btn: Button = $VBox/NewSaveBar/NewSaveButton
@onready var confirm_btn: Button = $VBox/ButtonBar/ConfirmButton
@onready var cancel_btn: Button = $VBox/ButtonBar/CancelButton
@onready var status_label: Label = $VBox/StatusLabel

var _mode: String = "save"  # "save" or "load"
var _selected_slot: String = ""
var _save_manager = null

func _ready():
	# Get SaveManager through Bootstrap autoload
	var bootstrap = get_node_or_null("/root/Bootstrap")
	# SaveManager would need to be in scene tree or autoload
	
	# Tab buttons
	if save_tab:
		save_tab.pressed.connect(func(): _set_mode("save"))
	if load_tab:
		load_tab.pressed.connect(func(): _set_mode("load"))
	
	# New save button
	if new_save_btn:
		new_save_btn.pressed.connect(_on_new_save)
	
	# Confirm/Cancel
	if confirm_btn:
		confirm_btn.pressed.connect(_on_confirm)
	if cancel_btn:
		cancel_btn.pressed.connect(func(): hide())
	
	# Start hidden
	hide()

func _set_mode(mode: String):
	_mode = mode
	if title_label:
		title_label.text = "SAVE" if mode == "save" else "LOAD"
	if new_save_input:
		new_save_input.visible = (mode == "save")
	if new_save_btn:
		new_save_btn.visible = (mode == "save")
	_refresh_slots()

func _refresh_slots():
	# Clear existing
	for child in slot_list.get_children():
		child.queue_free()
	
	# TODO: Get slots from SaveManager.Instance.GetSaveSlots()
	# For now, show placeholder
	var placeholder = Label.new()
	placeholder.text = "No save slots found."
	placeholder.modulate = Color(0.6, 0.6, 0.6)
	slot_list.add_child(placeholder)

func _on_new_save():
	if not new_save_input:
		return
	var name = new_save_input.text.strip_edges()
	if name.is_empty():
		if status_label:
			status_label.text = "Enter a save name."
		return
	_selected_slot = name
	_perform_save()

func _on_confirm():
	if _mode == "save" and _selected_slot != "":
		_perform_save()
	elif _mode == "load" and _selected_slot != "":
		_perform_load()

func _perform_save():
	# TODO: SaveManager.Instance.SaveGame(_selected_slot)
	if status_label:
		status_label.text = "Saving..."
	GD.Print("[SaveLoadMenu] Save requested: " + _selected_slot)

func _perform_load():
	# TODO: SaveManager.Instance.LoadGame(_selected_slot)
	if status_label:
		status_label.text = "Loading..."
	GD.Print("[SaveLoadMenu] Load requested: " + _selected_slot)

func _create_slot_item(slot_info) -> Control:
	var container = PanelContainer.new()
	
	var vbox = VBoxContainer.new()
	container.add_child(vbox)
	
	var name_label = Label.new()
	name_label.text = slot_info.get("SlotName", "Unknown") if slot_info.has_method("get") else "Unknown"
	vbox.add_child(name_label)
	
	var info_label = Label.new()
	var year = slot_info.get("Year", 0) if slot_info.has_method("get") else 0
	var month = slot_info.get("Month", 0) if slot_info.has_method("get") else 0
	var balance = slot_info.get("Balance", 0) if slot_info.has_method("get") else 0
	info_label.text = "Year %d, Month %d - R%.0f" % [year, month, balance]
	info_label.modulate = Color(0.7, 0.7, 0.7)
	vbox.add_child(info_label)
	
	container.gui_input.connect(func(event):
		if event is InputEventMouseButton and event.pressed:
			_selected_slot = slot_info.get("SlotName", "") if slot_info.has_method("get") else ""
	)
	
	return container
