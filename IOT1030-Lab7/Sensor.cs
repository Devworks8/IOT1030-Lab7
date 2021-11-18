/* Lab Question  
 * 
 * In this lab, we have split the refactored the Cell class by creating a new Sensor class
 * and moving much of the functionality of recording the temperature/flux 
 * measurements into this new class. This also enabled us to separate the link multiple cells to 
 * a single sensor so that we are not required to take measurements in each cell. This could
 * be a major performance issue if we want to use many tiny cells to build certain types of geometries.
 * 
 * Which SOLID principles are we doing our best to abide by here? 
 * Do you think this refactoring was worthwhile? 
 * Justify your answer by providing some upsides and downsides.
 *  
 */

using System;
using System.Collections.Generic;

using Psim.Materials;
using Psim.Particles;

namespace Psim.ModelComponents
{
	public class Sensor
	{
		private double heatCapacity;
		private List<double> temperatures = new() { };
		private List<double> xFluxes = new() { };
		private List<double> yFluxes = new() { };
		public int ID { get; }
		public double InitTemp { get; }
		public Material Material { get; }
		public Tuple<double, double>[] BaseTable { get; private set; }
		public Tuple<double, double>[] ScatterTable { get; private set; }
		public double HeatCapacity { get { return heatCapacity; } }
		public double Temperature { get; private set; }
		public double AreaCovered { get; private set; }

		public Sensor(int id, Material material, double initTemp)
		{
			ID = id;
			Material = material;
			InitTemp = initTemp;
			BaseTable = material.BaseData(initTemp, out heatCapacity);
			ScatterTable = material.ScatterTable(initTemp);
			Temperature = initTemp;
		}

		public void AddToArea(double area) => AreaCovered += area;
		public Tuple<double, double>[] GetEmitData(double temp, out double energy)
		{
			return Material.EmitData(temp, out energy);
		}

		public void TakeMeasurements(List<Phonon> phonons, double effEnergy, double tEq)
		{
			int energyUnits = 0;
			double xFlux = 0;
			double yFlux = 0;
			foreach (var p in phonons)
			{
				int sign = p.Sign;
				p.GetDirection(out double dx, out double dy);
				energyUnits += sign;
				xFlux += dx * p.Speed * sign;
				yFlux += dy * p.Speed * sign;
			}
			double fluxFactor = effEnergy / AreaCovered;

			temperatures.Add((energyUnits * effEnergy / (AreaCovered * HeatCapacity)) + tEq);
			xFluxes.Add(fluxFactor * xFlux);
			yFluxes.Add(fluxFactor * yFlux);
			UpdateParams();
		}

		public SensorMeasurements GetMeasurements()
		{
			SensorMeasurements measurements;
			measurements.InitTemp = InitTemp;
			measurements.Temperatures = temperatures;
			measurements.XFluxes = xFluxes;
			measurements.YFluxes = yFluxes;
			return measurements;
		}

		private void UpdateParams()
		{
			Temperature = temperatures[^1];
			BaseTable = Material.BaseData(Temperature, out heatCapacity);
			ScatterTable = Material.ScatterTable(Temperature);
		}
		public override string ToString() => $"Sensor {ID}: {Math.Round(Temperature, 2)}";
	}

	public struct SensorMeasurements
	{
		public double InitTemp;
		public List<double> Temperatures;
		public List<double> XFluxes;
		public List<double> YFluxes;
		public void Deconstruct(out List<double> temps, out List<double> xfs, out List<double> yfs)
		{
			temps = Temperatures;
			xfs = XFluxes;
			yfs = YFluxes;
		}
	}
}
