using Brightstone;

public class WorldTerrainMaterialEditor : BaseMaterialEditor
{
    protected override void OnRegisterOptions()
    {
        AddOption("VERTEX_COLOR", "Render vertex colors instead of texture sampling.");
    }
}
