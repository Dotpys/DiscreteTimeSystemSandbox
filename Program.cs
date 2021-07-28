using System;

namespace DynamicSystemPlayground
{
	class Program
	{
		static void Main(string[] args)
		{
			//  Creates a system that emulates the fibonacci sequence.
			//  Dynamic matrix A
			Matrix a = new Matrix("0 1;1 1");
			//  Input-States matrix B
			Matrix b = new Matrix("0;0");
			//  States-Output matrix C
			Matrix c = new Matrix("1 1");
			//  Input-Output matrix D
			Matrix d = new Matrix("0");

			//  Creates the system with the matrices we just created.
			var sys = new DiscreteTimeSystem(a, b, c, d);

			//  Initial states x_0
			Matrix x0 = new Matrix("1;0");

			//  Simulates the systen with the given initial states for the given number of timesteps.
			sys.lsim(x0, InputFunction, 100);
		}

		//  Calculates the u(k) inputs for the given timestep k.
		public static Matrix InputFunction(int k)
		{
			//  Change the function as needed by the simulation.
			return new Matrix("1");
		}

		//  Some signal function that can be used to create various types of inputs.
		public static float PulseFunction(float t)
		{
			return t == 0 ? 1 : 0;
		}

		public static float StepFunction(float t)
		{
			return t >= 0 ? 1 : 0;
		}

		public static float RampFunction(float t)
		{
			return t * StepFunction(t);
		}

		public static float ExpFunction(float t, float a)
		{
			return MathF.Exp(a*t) * StepFunction(t);
		}

		public static float SinFunction(float t, float omega)
		{
			return MathF.Sin(omega*t) * StepFunction(t);
		}

		public static float CosFunction(float t, float omega)
		{
			return MathF.Cos(omega*t) * StepFunction(t);
		}
	}


	class DiscreteTimeSystem
	{
		Matrix A, B, C, D;

		//  Creates a Discrete Time System with the A,B,C,D matrices.
		//  x(k+1) = Ax(k) + Bu(k)
		//  y(k) = Cx(k) + Du(k)
		public DiscreteTimeSystem(Matrix a, Matrix b, Matrix c, Matrix d)
		{
			if (!a.IsSquare())
				throw new ArgumentException("The A matrix should be a square matrix.");
			A = a;
			B = b;
			C = c;
			D = d;
		}

		//  Simulates the system with the given initial states and an input function for the specified amount of timesteps.
		public void lsim(Matrix initialStates, Func<int, Matrix> inputFunction, int steps)
		{
			Matrix x = initialStates;
			Matrix y = new Matrix(1, 1);
			for(int k=0; k<steps; k++)
			{
				//  Evaluates y(k).
				y = C * x + D * inputFunction(k);
				//  Evaluates x(k+1).
				x = A * x + B * inputFunction(k);

				Console.WriteLine(y);
			}
		}
	}

	class Matrix
	{
		int nRows;
		int nCols;
		double[,] data;

		//  Matrix definition by rows and columns count.
		public Matrix(int rows, int cols)
		{
			nRows = rows;
			nCols = cols;
			data = new double[rows, cols];
		}

		//  Matrix definition by its data, uses the matlab string format,
		//  ';' to separate rows, ' ' to separate elements of the same row.
		public Matrix(string format)
		{
			string[] rows = format.Split(';');
			nRows = rows.Length;
			for(int i=0; i<rows.Length; i++)
			{
				string[] values = rows[i].Split(' ');
				if (values.Length != 0 && nCols == 0)
				{
					nCols = values.Length;
					data = new double[nRows, nCols];
				} 
				if (values.Length == 0 || values.Length != nCols)
					throw new ArgumentException("Different number of elements from row to row.");
				for (int j = 0; j < values.Length; j++)
				{
					if (values[j] != string.Empty)
						data[i, j] = double.Parse(values[j]);
				}
			}
		}

		//  Defines the product operation between two matrices.
		public static Matrix operator *(Matrix a, Matrix b)
		{
			if (a.nCols != b.nRows)
				throw new ArgumentException("The A matrix must have the same number of columns as the number of rows of the B matrix.");
			Matrix result = new Matrix(a.nRows, b.nCols);
			for (int i=0; i<a.nRows; i++)
			{
				for (int j=0; j<b.nCols; j++)
				{
					double sum = 0;
					for (int c=0; c<a.nCols; c++)
					{
						sum += a.data[i, c] * b.data[c, j];
					}
					result.data[i, j] = sum;
				}
			}
			return result;
		}

		//  Defines the sum operation between two matrices.
		public static Matrix operator +(Matrix a, Matrix b)
		{
			if (a.nRows != b.nRows || a.nCols != b.nCols)
				throw new ArgumentException("Both matrices must have the same size.");
			Matrix result = new Matrix(a.nRows, a.nCols);
			for (int i = 0; i < a.nRows; i++)
			{
				for (int j = 0; j < b.nCols; j++)
				{
					result.data[i, j] = a.data[i, j] + b.data[i, j];
				}
			}
			return result;
		}

		//  Not very useful, works well only with 1x1 matrices, used just to display the y of the system above.
		//  TODO: Make this method work with any arbitrary matrix size.
		public override string ToString()
		{
			return data[0, 0].ToString();
		}

		//  Nice function, much effort.
		public bool IsSquare() => nRows == nCols;
	}
}