namespace FlexPlayer.Pool
{
	public interface IPoolable
	{
		public void Init();
		public bool IsActive();
		public void Activate();
		public void Deactivate();
		public IPoolable Duplicate();
	}
}