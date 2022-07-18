using Nuke.Common.IO;

public class Platform
{
    readonly AbsolutePath ParentDir;

    public Platform(AbsolutePath parentDir, string rid)
    {
        ParentDir = parentDir;
        Rid = rid;
    }
    
    public string Rid { get; }
    public AbsolutePath TargetDir => ParentDir / Rid;
}