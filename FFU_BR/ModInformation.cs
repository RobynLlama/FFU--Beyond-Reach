using System.IO;

namespace FFU_Beyond_Reach;

public class ModInformation(JsonModInfo mod, string directory, string aLoad)
{
  public readonly JsonModInfo Mod = mod;
  public readonly string ModDir = directory;
  public readonly string DataDir = Path.Combine(directory, "data");
  public readonly string aLoadEntry = aLoad;
}
