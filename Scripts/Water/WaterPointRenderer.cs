using Godot;
using BasterBoer.Core.Water;

namespace BasterBoer.Water
{
	/// <summary>
	/// Visual representation of a water source in the 3D world.
	/// Renders appropriate placeholder mesh based on WaterSourceType.
	/// Updates water level visual and shows repair indicator when damaged.
	/// Only renders when within player chunk range (sim/render split).
	/// </summary>
	public partial class WaterPointRenderer : Node3D
	{
		[Export] public WaterSourceType SourceType = WaterSourceType.Dam;
		[Export] public float WaterLevel = 0.8f;
		[Export] public bool IsOperational = true;
		[Export] public float QualityFactor = 0.9f;

		// Visual components (created in _Ready)
		private MeshInstance3D _baseMesh;
		private MeshInstance3D _waterMesh;
		private MeshInstance3D _repairIndicator;
		private Label3D _infoLabel;

		// Water material (animated color based on level/quality)
		private StandardMaterial3D _waterMaterial;
		private StandardMaterial3D _baseMaterial;

		/// <summary>The water source ID this renderer is tracking.</summary>
		public int WaterSourceId { get; set; } = -1;

		public override void _Ready()
		{
			CreateVisuals();
		}

		/// <summary>
		/// Creates placeholder geometry based on water source type.
		/// Dam = flat pool, Borehole = cylinder, Trough = small box.
		/// </summary>
		private void CreateVisuals()
		{
			// Base structure material
			_baseMaterial = new StandardMaterial3D
			{
				AlbedoColor = SourceType switch
				{
					WaterSourceType.Dam => new Color(0.35f, 0.3f, 0.2f),     // Earth brown
					WaterSourceType.Borehole => new Color(0.5f, 0.5f, 0.5f),  // Metal grey
					WaterSourceType.Trough => new Color(0.4f, 0.35f, 0.25f),  // Wood brown
					WaterSourceType.River => new Color(0.3f, 0.35f, 0.25f),   // River bank
					_ => new Color(0.4f, 0.4f, 0.4f)
				},
				Roughness = 0.8f
			};

			// Water material
			_waterMaterial = new StandardMaterial3D
			{
				AlbedoColor = new Color(0.15f, 0.4f, 0.7f, 0.8f),
				Transparency = BaseMaterial3D.TransparencyEnum.Alpha,
				Roughness = 0.1f,
				Metallic = 0.3f
			};

			switch (SourceType)
			{
				case WaterSourceType.Dam:
					CreateDamVisual();
					break;
				case WaterSourceType.Borehole:
					CreateBoreholeVisual();
					break;
				case WaterSourceType.Trough:
					CreateTroughVisual();
					break;
				default:
					CreateDamVisual();
					break;
			}

			// Repair indicator (hidden by default)
			_repairIndicator = new MeshInstance3D();
			var indicatorMesh = new SphereMesh { Radius = 0.3f, Height = 0.6f };
			_repairIndicator.Mesh = indicatorMesh;
			_repairIndicator.Position = new Vector3(0, 3f, 0);
			var indicatorMat = new StandardMaterial3D
			{
				AlbedoColor = new Color(1f, 0.2f, 0.2f),
				EmissionEnabled = true,
				Emission = new Color(1f, 0.1f, 0.1f)
			};
			_repairIndicator.SetSurfaceOverrideMaterial(0, indicatorMat);
			_repairIndicator.Visible = !IsOperational;
			AddChild(_repairIndicator);

			// Info label
			_infoLabel = new Label3D();
			_infoLabel.Position = new Vector3(0, 4f, 0);
			_infoLabel.Billboard = BaseMaterial3D.BillboardModeEnum.Enabled;
			_infoLabel.FontSize = 12;
			_infoLabel.Text = $"{SourceType}: {WaterLevel:P0}";
			AddChild(_infoLabel);
		}

		private void CreateDamVisual()
		{
			// Dam base (earth wall)
			_baseMesh = new MeshInstance3D();
			var baseMeshObj = new BoxMesh
			{
				Size = new Vector3(8f, 1.5f, 8f)
			};
			_baseMesh.Mesh = baseMeshObj;
			_baseMesh.SetSurfaceOverrideMaterial(0, _baseMaterial);
			_baseMesh.Position = new Vector3(0, 0.75f, 0);
			AddChild(_baseMesh);

			// Water surface
			_waterMesh = new MeshInstance3D();
			var waterMeshObj = new BoxMesh
			{
				Size = new Vector3(7f, 0.1f, 7f)
			};
			_waterMesh.Mesh = waterMeshObj;
			_waterMesh.SetSurfaceOverrideMaterial(0, _waterMaterial);
			_waterMesh.Position = new Vector3(0, 1.4f, 0);
			AddChild(_waterMesh);
		}

		private void CreateBoreholeVisual()
		{
			// Borehole casing (pipe)
			_baseMesh = new MeshInstance3D();
			var baseMeshObj = new CylinderMesh
			{
				TopRadius = 0.3f,
				BottomRadius = 0.3f,
				Height = 2.5f
			};
			_baseMesh.Mesh = baseMeshObj;
			_baseMesh.SetSurfaceOverrideMaterial(0, _baseMaterial);
			_baseMesh.Position = new Vector3(0, 1.25f, 0);
			AddChild(_baseMesh);

			// Small water puddle at base
			_waterMesh = new MeshInstance3D();
			var waterMeshObj = new CylinderMesh
			{
				TopRadius = 1.5f,
				BottomRadius = 1.5f,
				Height = 0.1f
			};
			_waterMesh.Mesh = waterMeshObj;
			_waterMesh.SetSurfaceOverrideMaterial(0, _waterMaterial);
			_waterMesh.Position = new Vector3(0, 0.05f, 0);
			AddChild(_waterMesh);
		}

		private void CreateTroughVisual()
		{
			// Trough container (box)
			_baseMesh = new MeshInstance3D();
			var baseMeshObj = new BoxMesh
			{
				Size = new Vector3(3f, 1f, 1.5f)
			};
			_baseMesh.Mesh = baseMeshObj;
			_baseMesh.SetSurfaceOverrideMaterial(0, _baseMaterial);
			_baseMesh.Position = new Vector3(0, 0.5f, 0);
			AddChild(_baseMesh);

			// Water inside trough
			_waterMesh = new MeshInstance3D();
			var waterMeshObj = new BoxMesh
			{
				Size = new Vector3(2.8f, 0.1f, 1.3f)
			};
			_waterMesh.Mesh = waterMeshObj;
			_waterMesh.SetSurfaceOverrideMaterial(0, _waterMaterial);
			_waterMesh.Position = new Vector3(0, 0.9f, 0);
			AddChild(_waterMesh);
		}

		/// <summary>
		/// Updates the visual representation to match current water source state.
		/// Called by WaterInteraction or SimulationTicker when state changes.
		/// </summary>
		public void UpdateVisual(float level, bool operational, float quality)
		{
			WaterLevel = level;
			IsOperational = operational;
			QualityFactor = quality;

			// Update water mesh scale based on level
			if (_waterMesh != null)
			{
				Vector3 baseScale = _waterMesh.Scale;
				_waterMesh.Scale = new Vector3(baseScale.X, Mathf.Max(0.01f, level), baseScale.Z);
				_waterMesh.Visible = level > 0.01f;
			}

			// Update water color based on quality
			if (_waterMaterial != null)
			{
				float blue = 0.3f + quality * 0.5f;
				float green = quality < 0.5f ? 0.5f - quality * 0.3f : 0.35f;
				_waterMaterial.AlbedoColor = new Color(0.1f, green, blue, 0.8f);
			}

			// Show/hide repair indicator
			if (_repairIndicator != null)
			{
				_repairIndicator.Visible = !operational;
			}

			// Update info label
			if (_infoLabel != null)
			{
				string status = operational ? "" : " [BROKEN]";
				_infoLabel.Text = $"{SourceType}: {level:P0}{status}";
			}
		}
	}
}
