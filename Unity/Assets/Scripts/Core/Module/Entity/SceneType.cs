namespace ET
{
	public enum SceneType
	{
		None = -1,
		Process = 0,
		Manager = 1,
		Realm = 2,
		Gate = 3,
		Http = 4,
		Location = 5,
		Map = 6,
		Router = 7,
		RouterManager = 8,
		Robot = 9,
		BenchmarkClient = 10,
		BenchmarkServer = 11,
		Benchmark = 12,
		
		Name = 13,		// 名字查重服
		Queue = 14,		// 排队服

		// 客户端Model层
		Client = 31,
		Current = 34,
		
		Max,
	}
}