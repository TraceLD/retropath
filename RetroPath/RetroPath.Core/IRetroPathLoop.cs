namespace RetroPath.Core;

public interface IRetroPathLoop<TResult>
{
    public List<TResult> Results { get; }
    public int I { get; }
    
    public void Run();
    public void RunIteration();
}