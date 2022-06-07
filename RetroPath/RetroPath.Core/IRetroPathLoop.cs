namespace RetroPath.Core;

public interface IRetroPathLoop<out TResult>
{
    public int I { get; }
    
    public TResult Run();
    public void RunIteration();
}