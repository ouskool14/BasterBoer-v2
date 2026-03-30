extends PanelContainer

## Toggleable event history panel. Shows events grouped by month.
## Press Tab to toggle. Filter by category.

@onready var title_label: Label = $VBox/Title
@onready var close_button: Button = $VBox/Title/CloseButton
@onready var filter_option: OptionButton = $VBox/FilterBar/FilterOption
@onready var event_list: VBoxContainer = $VBox/ScrollContainer/EventList

var _event_logger = null
var _is_open: bool = false

var CATEGORY_NAMES = ["Animals", "Economy", "Infrastructure", "Weather", "Staff", "System"]
var CATEGORY_COLORS = {
	0: Color(0.3, 0.6, 0.3),   # Animal - green
	1: Color(0.6, 0.5, 0.2),   # Economy - gold
	2: Color(0.4, 0.4, 0.5),   # Infrastructure - grey
	3: Color(0.3, 0.5, 0.7),   # Weather - blue
	4: Color(0.5, 0.3, 0.5),   # Staff - purple
	5: Color(0.5, 0.5, 0.5)    # System - neutral
}

func _ready():
	# Get event logger
	var bootstrap = get_node_or_null("/root/Bootstrap")
	if bootstrap:
		# EventLogger is a C# singleton
		pass
	
	# Setup filter options
	if filter_option:
		filter_option.add_item("All", -1)
		for i in range(CATEGORY_NAMES.size()):
			filter_option.add_item(CATEGORY_NAMES[i], i)
		filter_option.item_selected.connect(_on_filter_changed)
	
	# Close button
	if close_button:
		close_button.pressed.connect(func(): toggle())
	
	# Start hidden
	hide()

func _input(event):
	if event is InputEventKey and event.pressed and event.keycode == KEY_TAB:
		toggle()

func toggle():
	_is_open = !_is_open
	if _is_open:
		refresh_events()
		show()
	else:
		hide()

func refresh_events():
	# Clear existing items
	for child in event_list.get_children():
		child.queue_free()
	
	# TODO: Connect to EventLogger C# singleton and populate
	# For now, show placeholder
	var placeholder = Label.new()
	placeholder.text = "Event log will populate from simulation events."
	placeholder.modulate = Color(0.6, 0.6, 0.6)
	event_list.add_child(placeholder)

func _on_filter_changed(index: int):
	refresh_events()

func add_event_to_list(category: int, summary: String, timestamp: String):
	var container = HBoxContainer.new()
	
	# Category color indicator
	var indicator = ColorRect.new()
	indicator.custom_minimum_size = Vector2(4, 0)
	indicator.color = CATEGORY_COLORS.get(category, Color.GRAY)
	container.add_child(indicator)
	
	# Timestamp
	var time_label = Label.new()
	time_label.text = timestamp
	time_label.custom_minimum_size = Vector2(100, 0)
	time_label.modulate = Color(0.7, 0.7, 0.7)
	container.add_child(time_label)
	
	# Summary
	var summary_label = Label.new()
	summary_label.text = summary
	summary_label.size_flags_horizontal = Control.SIZE_EXPAND_FILL
	container.add_child(summary_label)
	
	event_list.add_child(container)
