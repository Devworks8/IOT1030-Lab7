using Psim.Materials;
using Psim.IOManagers;

namespace Psim
{
	class Program
	{
		static void Main(string[] args)
		{
#if true
			DispersionData dData;
			dData.LaData = new double[] { -2.22e-7, 9260.0, 0.0};
			dData.TaData = new double[] { -2.28e-7, 5240.0, 0.0};
			dData.WMaxLa = 7.63916048e13;
			dData.WMaxTa = 3.0100793072e13;

			RelaxationData rData;
			rData.Bl = 1.3e-24;
			rData.Btn = 9e-13;
			rData.Btu = 1.9e-18;
			rData.BI = 1.2e-45;
			rData.W = 2.42e13;

			// Model specification
			const int NUM_CELLS = 40;
			const double SIM_TIME = 1e-9;
			const double T_HIGH = 310;
			const double T_LOW = 290;
			const double T_INIT = (T_HIGH + T_LOW) / 2;
			const double CELL_LENGTH = 50e-9;
			const double CELL_WIDTH = 10e-9;
			Material silicon = new Material(in dData, in rData);

			Model model = new Model(silicon, T_HIGH, T_LOW, SIM_TIME);
			// Add sensors & cells to the model
			for (int i = 0; i < NUM_CELLS; ++i)
			{
				model.AddSensor(i, T_INIT);
				model.AddCell(CELL_LENGTH, CELL_WIDTH, i);
			}

			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			model.RunSimulation();

			watch.Stop();
			System.Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000} [s]");
#else

			const string path = "../../../model.json";
			Model model = InputManager.InitializeModel(path);

			var watch = new System.Diagnostics.Stopwatch();
			watch.Start();

			model.RunSimulation();
			System.Console.WriteLine(model);

			watch.Stop();
			System.Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds / 1000} [s]");
#endif
		}
	}
}
