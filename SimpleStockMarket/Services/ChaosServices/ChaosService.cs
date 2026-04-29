namespace Services;

public class ChaosService : IChaosService
{
	public void Terminate(int exitCode)
    {
        Environment.Exit(exitCode);
    }
}