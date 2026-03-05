using System;

namespace MatrixCalculator {
  // Пользовательские исключения
  public class MatrixException : Exception {
    public MatrixException() : base() { }
    public MatrixException(string message) : base(message) { }
    public MatrixException(string message, Exception innerException) : base(message, innerException) { }
  }

  public class MatrixSizeException : MatrixException {
    public MatrixSizeException() : base() { }
    public MatrixSizeException(string message) : base(message) { }
    public MatrixSizeException(string message, Exception innerException) : base(message, innerException) { }
  }

  public class MatrixSingularException : MatrixException {
    public MatrixSingularException() : base() { }
    public MatrixSingularException(string message) : base(message) { }
    public MatrixSingularException(string message, Exception innerException) : base(message, innerException) { }
  }

  // Квадратная матрица
  public class SquareMatrix : IComparable, ICloneable {
    private const double DefaultEps = 1e-12;
    private const double DefaultMinRandomValue = -5.0;
    private const double DefaultMaxRandomValue = 5.0;

    public int Size { get; private set; }
    private double[,] matrixData;

    // Индексатор
    public double this[int rowIndex, int columnIndex] {
      get { return matrixData[rowIndex, columnIndex]; }
      set { matrixData[rowIndex, columnIndex] = value; }
    }

    // Конструкторы
    public SquareMatrix() {
      Size = 0;
      matrixData = new double[0, 0];
    }

    public SquareMatrix(int size) {
      if (size <= 0)
        throw new MatrixSizeException("Matrix size must be greater than zero.");

      Size = size;
      matrixData = new double[size, size];
    }

    // Конструктор случайной матрицы с диапазоном по умолчанию
    public SquareMatrix(int size, Random random)
        : this(size, DefaultMinRandomValue, DefaultMaxRandomValue, random) {
    }

    // Конструктор случайной матрицы с заданным диапазоном
    public SquareMatrix(int size, double minValue, double maxValue, Random random)
        : this(size) {
      for (int rowIndex = 0; rowIndex < size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < size; ++columnIndex)
          matrixData[rowIndex, columnIndex] =
              minValue + random.NextDouble() * (maxValue - minValue);
    }

    // Конструктор из двумерного массива
    public SquareMatrix(double[,] sourceArray) {
      if (sourceArray == null)
        throw new MatrixException("Source array cannot be null.");

      int rowCount = sourceArray.GetLength(0);
      int columnCount = sourceArray.GetLength(1);

      if (rowCount != columnCount)
        throw new MatrixSizeException("Matrix must be square.");

      Size = rowCount;
      matrixData = new double[Size, Size];
      Array.Copy(sourceArray, matrixData, sourceArray.Length);
    }

    // Норма (аналог Abs у Complex)
    public double Norm {
      get {
        double sumOfSquares = 0;
        for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
          for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
            sumOfSquares += matrixData[rowIndex, columnIndex] * matrixData[rowIndex, columnIndex];
        return Math.Sqrt(sumOfSquares);
      }
    }

    //Перегрузка операций сложения / вычитания / умножения 

    public static SquareMatrix operator +(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix.Size != rightMatrix.Size)
        throw new MatrixSizeException("Cannot add matrices of different sizes.");

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] + rightMatrix[rowIndex, columnIndex];

      return resultMatrix;
    }

    public static SquareMatrix operator -(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix.Size != rightMatrix.Size)
        throw new MatrixSizeException("Cannot subtract matrices of different sizes.");

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] - rightMatrix[rowIndex, columnIndex];

      return resultMatrix;
    }

    // Матрица + число
    public static SquareMatrix operator +(SquareMatrix leftMatrix, double rightValue) {
      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] + rightValue;
      return resultMatrix;
    }

    public static SquareMatrix operator -(SquareMatrix leftMatrix, double rightValue) {
      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] - rightValue;
      return resultMatrix;
    }

    public static SquareMatrix operator +(double leftValue, SquareMatrix rightMatrix) {
      return rightMatrix + leftValue;
    }

    public static SquareMatrix operator -(double leftValue, SquareMatrix rightMatrix) {
      SquareMatrix resultMatrix = new SquareMatrix(rightMatrix.Size);
      for (int rowIndex = 0; rowIndex < rightMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < rightMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftValue - rightMatrix[rowIndex, columnIndex];
      return resultMatrix;
    }

    // Умножение матриц
    public static SquareMatrix operator *(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix.Size != rightMatrix.Size)
        throw new MatrixSizeException("Cannot multiply matrices of different sizes.");

      int matrixSize = leftMatrix.Size;
      SquareMatrix resultMatrix = new SquareMatrix(matrixSize);

      for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex)
        for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
          double elementSum = 0;
          for (int innerIndex = 0; innerIndex < matrixSize; ++innerIndex)
            elementSum += leftMatrix[rowIndex, innerIndex] * rightMatrix[innerIndex, columnIndex];

          resultMatrix[rowIndex, columnIndex] = elementSum;
        }

      return resultMatrix;
    }

    // Матрица * число / деление на число
    public static SquareMatrix operator *(SquareMatrix leftMatrix, double rightValue) {
      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] * rightValue;
      return resultMatrix;
    }

    public static SquareMatrix operator *(double leftValue, SquareMatrix rightMatrix) {
      return rightMatrix * leftValue;
    }

    public static SquareMatrix operator /(SquareMatrix leftMatrix, double rightValue) {
      if (Math.Abs(rightValue) < DefaultEps)
        throw new DivideByZeroException("Division of a matrix by zero.");

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.Size);
      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] / rightValue;
      return resultMatrix;
    }

    // Операции сравнения (==, !=, >, <, >=, <=) 

    public static bool operator ==(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (ReferenceEquals(leftMatrix, rightMatrix)) return true;
      if ((object)leftMatrix == null || (object)rightMatrix == null) return false;

      if (leftMatrix.Size != rightMatrix.Size) return false;

      for (int rowIndex = 0; rowIndex < leftMatrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < leftMatrix.Size; ++columnIndex)
          if (leftMatrix[rowIndex, columnIndex] != rightMatrix[rowIndex, columnIndex])
            return false;

      return true;
    }

    public static bool operator !=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return !(leftMatrix == rightMatrix);
    }

    // Сравнение по Norm
    public static bool operator >(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return leftMatrix.Norm > rightMatrix.Norm;
    }

    public static bool operator <(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return leftMatrix.Norm < rightMatrix.Norm;
    }

    public static bool operator >=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return leftMatrix.Norm >= rightMatrix.Norm;
    }

    public static bool operator <=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return leftMatrix.Norm <= rightMatrix.Norm;
    }

    // Equals, GetHashCode

    public override bool Equals(object otherObject) {
      bool areEqual = false;
      if (otherObject is SquareMatrix) {
        SquareMatrix otherMatrix = otherObject as SquareMatrix;
        if (otherMatrix.Size == this.Size) {
          areEqual = true;
          for (int rowIndex = 0; rowIndex < Size && areEqual; ++rowIndex)
            for (int columnIndex = 0; columnIndex < Size && areEqual; ++columnIndex)
              if (otherMatrix[rowIndex, columnIndex] != this[rowIndex, columnIndex])
                areEqual = false;
        }
      }
      return areEqual;
    }

    public override int GetHashCode() {
      return (int)Norm;
    }

    // IComparable.CompareTo 

    int IComparable.CompareTo(object otherObject) {
      if (otherObject is SquareMatrix) {
        SquareMatrix otherMatrix = otherObject as SquareMatrix;
        if (otherMatrix.Norm > this.Norm) return -1;
        if (otherMatrix.Norm == this.Norm) return 0;
        if (otherMatrix.Norm < this.Norm) return 1;
      }
      return -1;
    }

    // ToString 

    public override string ToString() {
      string stringResult = "";
      for (int rowIndex = 0; rowIndex < Size; ++rowIndex) {
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
          stringResult += matrixData[rowIndex, columnIndex]
              .ToString("0.###").PadLeft(8);
        stringResult += Environment.NewLine;
      }
      return stringResult;
    }

    //Операции преобразования типов

    public static implicit operator SquareMatrix(double[,] sourceArray) {
      return new SquareMatrix(sourceArray);
    }

    public static explicit operator string(SquareMatrix matrix) {
      return matrix.ToString();
    }

    public static explicit operator double(SquareMatrix matrix) {
      return matrix.Determinant();
    }

    // Булевские операции true / false 

    public static bool operator true(SquareMatrix matrix) {
      for (int rowIndex = 0; rowIndex < matrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < matrix.Size; ++columnIndex)
          if (matrix[rowIndex, columnIndex] != 0)
            return true;
      return false;
    }

    public static bool operator false(SquareMatrix matrix) {
      for (int rowIndex = 0; rowIndex < matrix.Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < matrix.Size; ++columnIndex)
          if (matrix[rowIndex, columnIndex] != 0)
            return false;
      return true;
    }

    // Детерминант и обратная матрица 

    public double Determinant() {
      if (Size == 0)
        throw new MatrixException("Matrix is empty.");

      int matrixSize = Size;
      double[,] workingArray = new double[matrixSize, matrixSize];
      Array.Copy(matrixData, workingArray, matrixData.Length);

      double determinant = 1.0;

      for (int pivotIndex = 0; pivotIndex < matrixSize; ++pivotIndex) {
        int maxRowIndex = pivotIndex;
        for (int rowIndex = pivotIndex + 1; rowIndex < matrixSize; ++rowIndex)
          if (Math.Abs(workingArray[rowIndex, pivotIndex]) >
              Math.Abs(workingArray[maxRowIndex, pivotIndex]))
            maxRowIndex = rowIndex;

        if (Math.Abs(workingArray[maxRowIndex, pivotIndex]) < DefaultEps)
          return 0.0;

        if (maxRowIndex != pivotIndex) {
          for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
            double temporaryValue = workingArray[pivotIndex, columnIndex];
            workingArray[pivotIndex, columnIndex] = workingArray[maxRowIndex, columnIndex];
            workingArray[maxRowIndex, columnIndex] = temporaryValue;
          }
          determinant = -determinant;
        }

        determinant *= workingArray[pivotIndex, pivotIndex];

        for (int rowIndex = pivotIndex + 1; rowIndex < matrixSize; ++rowIndex) {
          double eliminationFactor = workingArray[rowIndex, pivotIndex] /
                                     workingArray[pivotIndex, pivotIndex];
          for (int columnIndex = pivotIndex; columnIndex < matrixSize; ++columnIndex)
            workingArray[rowIndex, columnIndex] -=
                eliminationFactor * workingArray[pivotIndex, columnIndex];
        }
      }

      return determinant;
    }

    public SquareMatrix Inverse() {
      int matrixSize = Size;
      if (matrixSize == 0)
        throw new MatrixException("Matrix is empty.");

      double determinant = Determinant();
      if (Math.Abs(determinant) < DefaultEps)
        throw new MatrixSingularException("Matrix is singular, inverse does not exist.");

      double[,] workingArray = new double[matrixSize, matrixSize];
      double[,] inverseArray = new double[matrixSize, matrixSize];

      Array.Copy(matrixData, workingArray, matrixData.Length);
      for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex)
        inverseArray[rowIndex, rowIndex] = 1.0;

      for (int pivotIndex = 0; pivotIndex < matrixSize; ++pivotIndex) {
        int maxRowIndex = pivotIndex;
        for (int rowIndex = pivotIndex + 1; rowIndex < matrixSize; ++rowIndex)
          if (Math.Abs(workingArray[rowIndex, pivotIndex]) >
              Math.Abs(workingArray[maxRowIndex, pivotIndex]))
            maxRowIndex = rowIndex;

        if (Math.Abs(workingArray[maxRowIndex, pivotIndex]) < DefaultEps)
          throw new MatrixSingularException("Matrix is singular, inverse does not exist.");

        if (maxRowIndex != pivotIndex) {
          for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
            double temporaryValue = workingArray[pivotIndex, columnIndex];
            workingArray[pivotIndex, columnIndex] = workingArray[maxRowIndex, columnIndex];
            workingArray[maxRowIndex, columnIndex] = temporaryValue;

            temporaryValue = inverseArray[pivotIndex, columnIndex];
            inverseArray[pivotIndex, columnIndex] = inverseArray[maxRowIndex, columnIndex];
            inverseArray[maxRowIndex, columnIndex] = temporaryValue;
          }
        }

        double diagonalValue = workingArray[pivotIndex, pivotIndex];
        for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
          workingArray[pivotIndex, columnIndex] /= diagonalValue;
          inverseArray[pivotIndex, columnIndex] /= diagonalValue;
        }

        for (int rowIndex = 0; rowIndex < matrixSize; ++rowIndex) {
          if (rowIndex == pivotIndex) continue;
          double eliminationFactor = workingArray[rowIndex, pivotIndex];
          for (int columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
            workingArray[rowIndex, columnIndex] -=
                eliminationFactor * workingArray[pivotIndex, columnIndex];
            inverseArray[rowIndex, columnIndex] -=
                eliminationFactor * inverseArray[pivotIndex, columnIndex];
          }
        }
      }

      return new SquareMatrix(inverseArray);
    }

    // Прототип (глубокое копирование) 

    public object Clone() {
      SquareMatrix resultMatrix = new SquareMatrix(Size);
      for (int rowIndex = 0; rowIndex < Size; ++rowIndex)
        for (int columnIndex = 0; columnIndex < Size; ++columnIndex)
          resultMatrix[rowIndex, columnIndex] = this[rowIndex, columnIndex];
      return resultMatrix;
    }

    public SquareMatrix DeepCopy() {
      return (SquareMatrix)this.Clone();
    }
  }

  // Тестовое приложение «Матричный калькулятор»
  class Program {
    private const double MinRandomValue = -5.0;
    private const double MaxRandomValue = 5.0;

    static void Main(string[] args) {
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      try {
        Console.Write("Enter matrix size n: ");
        int matrixSize = int.Parse(Console.ReadLine() ?? "0");

        Random random = new Random();

        SquareMatrix firstMatrix = new SquareMatrix(matrixSize, MinRandomValue, MaxRandomValue, random);
        SquareMatrix secondMatrix = new SquareMatrix(matrixSize, MinRandomValue, MaxRandomValue, random);

        Console.WriteLine("\nMatrix A:");
        Console.WriteLine(firstMatrix.ToString());

        Console.WriteLine("Matrix B:");
        Console.WriteLine(secondMatrix.ToString());

        Console.WriteLine("A + B:");
        Console.WriteLine((firstMatrix + secondMatrix).ToString());

        Console.WriteLine("A - B:");
        Console.WriteLine((firstMatrix - secondMatrix).ToString());

        Console.WriteLine("A * B:");
        Console.WriteLine((firstMatrix * secondMatrix).ToString());

        Console.WriteLine("A + 1:");
        Console.WriteLine((firstMatrix + 1.0).ToString());

        Console.WriteLine("2 * B:");
        Console.WriteLine((2.0 * secondMatrix).ToString());

        Console.WriteLine("\nComparison by norm:");
        Console.WriteLine(
          "A == B : " + (firstMatrix == secondMatrix) + Environment.NewLine +
          "A != B : " + (firstMatrix != secondMatrix) + Environment.NewLine +
          "A >  B : " + (firstMatrix > secondMatrix) + Environment.NewLine +
          "A <  B : " + (firstMatrix < secondMatrix) + Environment.NewLine +
          "A >= B : " + (firstMatrix >= secondMatrix) + Environment.NewLine +
          "A <= B : " + (firstMatrix <= secondMatrix)
        );

        Console.WriteLine("\nDeterminants:");
        Console.WriteLine("det(A) = " + ((double)firstMatrix).ToString("0.###"));
        Console.WriteLine("det(B) = " + ((double)secondMatrix).ToString("0.###"));

        Console.WriteLine("\nInverse matrix A (if exists):");
        try {
          SquareMatrix inverseFirstMatrix = firstMatrix.Inverse();
          Console.WriteLine(inverseFirstMatrix.ToString());
        }
        catch (MatrixSingularException singularException) {
          Console.WriteLine("Error: " + singularException.Message);
        }

        Console.WriteLine("\nChecking true / false operators:");
        if (firstMatrix)
          Console.WriteLine("Matrix A is considered 'true' (has non-zero elements).");
        else
          Console.WriteLine("Matrix A is considered 'false' (all elements are zero).");

        Console.WriteLine("\nPrototype (deep copy):");
        SquareMatrix copyMatrix = firstMatrix.DeepCopy();
        Console.WriteLine("Copy of A:");
        Console.WriteLine(copyMatrix.ToString());

        if (matrixSize > 0)
          copyMatrix[0, 0] = 999;

        Console.WriteLine("Original A:");
        Console.WriteLine(firstMatrix.ToString());
        Console.WriteLine("Modified copy of A:");
        Console.WriteLine(copyMatrix.ToString());
      }
      catch (MatrixException matrixException) {
        Console.WriteLine("Matrix error: " + matrixException.Message);
      }
      catch (FormatException) {
        Console.WriteLine("Number input error.");
      }
      catch (Exception unexpectedException) {
        Console.WriteLine("Unknown error: " + unexpectedException.Message);
      }

      Console.WriteLine("\nPress any key to exit...");
      Console.ReadKey();
    }
  }
}
