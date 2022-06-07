namespace RetroPath.Core;

public interface IRetroPathLoop<out TResult>
{
    public int CurrentIteration { get; }
    
    public TResult Run();
    public void RunIteration();
}