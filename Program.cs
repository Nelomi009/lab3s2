using System;

namespace MatrixCalculator {
  // Пользовательские исключения
  public class MatrixException : Exception {
    public MatrixException() : base() { }
    public MatrixException(string message) : base(message) { }
    public MatrixException(string message, Exception innerException)
      : base(message, innerException) { }
  }

  public class MatrixSizeException : MatrixException {
    public MatrixSizeException() : base() { }
    public MatrixSizeException(string message) : base(message) { }
    public MatrixSizeException(string message, Exception innerException)
      : base(message, innerException) { }
  }

  // Квадратная матрица
  public class SquareMatrix : IComparable, ICloneable {
    private static double s_comparisonTolerance = 1e-12;
    private static double s_defaultMinRandomValue = -5.0;
    private static double s_defaultMaxRandomValue = 5.0;

    private double[,] _matrixData;

    public int size { get; private set; }

    // Индексатор
    public double this[int rowIndex, int columnIndex] {
      get {
        return _matrixData[rowIndex, columnIndex];
      }
      set {
        _matrixData[rowIndex, columnIndex] = value;
      }
    }

    // Конструкторы
    public SquareMatrix() {
      size = 0;  
      _matrixData = new double[0, 0];
    }

    public SquareMatrix(int size) {
      if (size <= 0) {
        throw new MatrixSizeException("Matrix size must be greater than zero.");
      }

      this.size = size;
      _matrixData = new double[size, size];
    }

    // Конструктор случайной матрицы с диапазоном по умолчанию
    public SquareMatrix(int size, Random randomGenerator)
      : this(size, s_defaultMinRandomValue, s_defaultMaxRandomValue, randomGenerator) {
    }

    // Конструктор случайной матрицы с заданным диапазоном
    public SquareMatrix(int size, double minValue, double maxValue, Random randomGenerator)
        : this(size) {
      if (randomGenerator == null) {
        throw new ArgumentNullException(nameof(randomGenerator));
      }

      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < size; ++columnIndex) {
          _matrixData[rowIndex, columnIndex] =
          minValue + randomGenerator.NextDouble() * (maxValue - minValue);
        }
      }
    }

    // Конструктор из двумерного массива
    public SquareMatrix(double[,] sourceArray) {
      if (sourceArray == null) {
        throw new ArgumentNullException(nameof(sourceArray));
      }

      int rowCount = sourceArray.GetLength(0);
      int columnCount = sourceArray.GetLength(1);

      if (rowCount != columnCount) {
        throw new MatrixSizeException("Matrix must be square.");
      }

      size = rowCount;
      _matrixData = new double[size, size];
      Array.Copy(sourceArray, _matrixData, sourceArray.Length);
    }

    // Свойство Norm 
    public double Norm {
      get {
        double sumOfSquares = 0.0;
        int rowIndex = 0;
        int columnIndex = 0;

        for (rowIndex = 0; rowIndex < size; ++rowIndex) {
          for (columnIndex = 0; columnIndex < size; ++columnIndex) {
            sumOfSquares += _matrixData[rowIndex, columnIndex] *
            _matrixData[rowIndex, columnIndex];
          }
        }

        return Math.Sqrt(sumOfSquares);
      }
    }

    // Перегрузка операций сложения
    public static SquareMatrix operator +(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      if (rightMatrix is null) {
        throw new ArgumentNullException(nameof(rightMatrix));
      }

      if (leftMatrix.size != rightMatrix.size) {
        throw new MatrixSizeException("Cannot add matrices of different sizes.");
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
          leftMatrix[rowIndex, columnIndex] + rightMatrix[rowIndex, columnIndex];
        }
      }

      return resultMatrix;
    }

    public static SquareMatrix operator -(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      if (rightMatrix is null) {
        throw new ArgumentNullException(nameof(rightMatrix));
      }

      if (leftMatrix.size != rightMatrix.size) {
        throw new MatrixSizeException("Cannot subtract matrices of different sizes.");
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] - rightMatrix[rowIndex, columnIndex];
        }
      }

      return resultMatrix;
    }

    // Матрица + число
    public static SquareMatrix operator +(SquareMatrix leftMatrix, double rightValue) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
          leftMatrix[rowIndex, columnIndex] + rightValue;
        }
      }

      return resultMatrix;
    }

    public static SquareMatrix operator -(SquareMatrix leftMatrix, double rightValue) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
          leftMatrix[rowIndex, columnIndex] - rightValue;
        }
      }

      return resultMatrix;
    }

    // Коммутативные операторы
    public static SquareMatrix operator +(double leftValue, SquareMatrix rightMatrix) {
      return rightMatrix + leftValue;
    }

    public static SquareMatrix operator -(double leftValue, SquareMatrix rightMatrix) {
      if (rightMatrix is null) {
        throw new ArgumentNullException(nameof(rightMatrix));
      }

      SquareMatrix resultMatrix = new SquareMatrix(rightMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < rightMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < rightMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
          leftValue - rightMatrix[rowIndex, columnIndex];
        }
      }

      return resultMatrix;
    }

    // Умножение матриц
    public static SquareMatrix operator *(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      if (rightMatrix is null) {
        throw new ArgumentNullException(nameof(rightMatrix));
      }

      if (leftMatrix.size != rightMatrix.size) {
        throw new MatrixSizeException("Cannot multiply matrices of different sizes.");
      }

      int matrixSize = leftMatrix.size;
      SquareMatrix resultMatrix = new SquareMatrix(matrixSize);

      int rowIndex = 0;
      int columnIndex = 0;
      int innerIndex = 0;

      for (rowIndex = 0; rowIndex < matrixSize; ++rowIndex) {
        for (columnIndex = 0; columnIndex < matrixSize; ++columnIndex) {
          double elementSum = 0.0;

          for (innerIndex = 0; innerIndex < matrixSize; ++innerIndex) {
            elementSum += leftMatrix[rowIndex, innerIndex] *
                          rightMatrix[innerIndex, columnIndex];
          }

          resultMatrix[rowIndex, columnIndex] = elementSum;
        }
      }

      return resultMatrix;
    }

    // Матрица * число
    public static SquareMatrix operator *(SquareMatrix leftMatrix, double rightValue) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] * rightValue;
        }
      }

      return resultMatrix;
    }

    public static SquareMatrix operator *(double leftValue, SquareMatrix rightMatrix) {
      return rightMatrix * leftValue;
    }

    // Деление на число
    public static SquareMatrix operator /(SquareMatrix leftMatrix, double rightValue) {
      if (leftMatrix is null) {
        throw new ArgumentNullException(nameof(leftMatrix));
      }

      if (Math.Abs(rightValue) < s_comparisonTolerance) {
        throw new DivideByZeroException("Division of a matrix by zero.");
      }

      SquareMatrix resultMatrix = new SquareMatrix(leftMatrix.size);
      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] =
              leftMatrix[rowIndex, columnIndex] / rightValue;
        }
      }

      return resultMatrix;
    }

    // Операции сравнения
    public static bool operator ==(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (ReferenceEquals(leftMatrix, rightMatrix)) {
        return true;
      }

      if (leftMatrix is null || rightMatrix is null) {
        return false;
      }

      if (leftMatrix.size != rightMatrix.size) {
        return false;
      }

      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < leftMatrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < leftMatrix.size; ++columnIndex) {
          if (Math.Abs(leftMatrix[rowIndex, columnIndex] -
                       rightMatrix[rowIndex, columnIndex]) > s_comparisonTolerance) {
            return false;
          }
        }
      }

      return true;
    }

    public static bool operator !=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      return !(leftMatrix == rightMatrix);
    }

    // Сравнение по Norm
    public static bool operator >(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null || rightMatrix is null) {
        return false;
      }

      return leftMatrix.Norm > rightMatrix.Norm;
    }

    public static bool operator <(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null || rightMatrix is null) {
        return false;
      }

      return leftMatrix.Norm < rightMatrix.Norm;
    }

    public static bool operator >=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null || rightMatrix is null) {
        return false;
      }

      return leftMatrix.Norm >= rightMatrix.Norm;
    }

    public static bool operator <=(SquareMatrix leftMatrix, SquareMatrix rightMatrix) {
      if (leftMatrix is null || rightMatrix is null) {
        return false;
      }

      return leftMatrix.Norm <= rightMatrix.Norm;
    }

    // Переопределение Equals
    public override bool Equals(object otherObject) {
      if (ReferenceEquals(this, otherObject)) {
        return true;
      }

      if (otherObject is SquareMatrix otherMatrix) {
        return this == otherMatrix;
      }

      return false;
    }

    // Переопределение GetHashCode
    public override int GetHashCode() {
      return (int)Norm ^ size.GetHashCode();
    }

    // IComparable.CompareTo
    public int CompareTo(object otherObject) {
      if (otherObject is null) {
        return 1;
      }

      if (otherObject is SquareMatrix otherMatrix) {
        if (this.Norm < otherMatrix.Norm) {
          return -1;
        }

        if (this.Norm > otherMatrix.Norm) {
          return 1;
        }

        return 0;
      }

      throw new ArgumentException("Object is not a SquareMatrix");
    }

    // ToString
    public override string ToString() {
      string resultString = string.Empty;

      int rowIndex = 0;
      int columnIndex = 0;

      int lastRowIndex = size - 1;

      for (rowIndex = 0; rowIndex < size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < size; ++columnIndex) {
          resultString += _matrixData[rowIndex, columnIndex]
              .ToString("0.###").PadLeft(8);
        }

        if (rowIndex < lastRowIndex) {
          resultString += Environment.NewLine;
        }
      }

      return resultString;
    }

    // Операции преобразования типов
    public static explicit operator string(SquareMatrix matrix) {
      if (matrix is null) {
        throw new ArgumentNullException(nameof(matrix));
      }

      return matrix.ToString();
    }

    public static implicit operator SquareMatrix(double[,] sourceArray) {
      return new SquareMatrix(sourceArray);
    }

    public static implicit operator SquareMatrix(double value) {
      SquareMatrix result = new SquareMatrix(1);
      result[0, 0] = value;
      return result;
    }

    // Булевские операции true/false
    public static bool operator true(SquareMatrix matrix) {
      if (matrix is null) {
        return false;
      }

      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < matrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < matrix.size; ++columnIndex) {
          if (Math.Abs(matrix[rowIndex, columnIndex]) > s_comparisonTolerance) {
            return true;
          }
        }
      }

      return false;
    }

    public static bool operator false(SquareMatrix matrix) {
      if (matrix is null) {
        return true;
      }

      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < matrix.size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < matrix.size; ++columnIndex) {
          if (Math.Abs(matrix[rowIndex, columnIndex]) > s_comparisonTolerance) {
            return false;
          }
        }
      }

      return true;
    }

    // ICloneable
    public object Clone() {
      SquareMatrix resultMatrix = new SquareMatrix(size);

      int rowIndex = 0;
      int columnIndex = 0;

      for (rowIndex = 0; rowIndex < size; ++rowIndex) {
        for (columnIndex = 0; columnIndex < size; ++columnIndex) {
          resultMatrix[rowIndex, columnIndex] = this[rowIndex, columnIndex];
        }
      }

      return resultMatrix;
    }

    public SquareMatrix DeepCopy() {
      return (SquareMatrix)this.Clone();
    }
  }

  // Тестовое приложение
  class Program {
    static void Main(string[] args) {
      Console.OutputEncoding = System.Text.Encoding.UTF8;

      try {
        Console.Write("Enter matrix size n: ");
        string inputString = Console.ReadLine() ?? "0";

        if (!int.TryParse(inputString, out int matrixSize) || matrixSize <= 0) {
          Console.WriteLine("Invalid input. Please enter a positive integer.");
          return;
        }

        Random randomGenerator = new Random();

        double minRandomValue = -5.0;
        double maxRandomValue = 5.0;
        double valueForAddition = 1.0;
        double valueForMultiplication = 2.0;
        double testValueForDeepCopy = 999.0;
        double valueForImplicitConversion = 5.0;

        SquareMatrix firstMatrix = new SquareMatrix(
            matrixSize, minRandomValue, maxRandomValue, randomGenerator);

        SquareMatrix secondMatrix = new SquareMatrix(
            matrixSize, minRandomValue, maxRandomValue, randomGenerator);

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

        Console.WriteLine($"A + {valueForAddition}:");
        Console.WriteLine((firstMatrix + valueForAddition).ToString());

        Console.WriteLine($"{valueForMultiplication} * B:");
        Console.WriteLine((valueForMultiplication * secondMatrix).ToString());

        Console.WriteLine("\nComparison by norm:\n" +
          $"A == B : {firstMatrix == secondMatrix}\n" +
          $"A != B : {firstMatrix != secondMatrix}\n" +
          $"A >  B : {firstMatrix > secondMatrix}\n" +
          $"A <  B : {firstMatrix < secondMatrix}\n" +
          $"A >= B : {firstMatrix >= secondMatrix}\n" +
          $"A <= B : {firstMatrix <= secondMatrix}");

        Console.WriteLine("\nEquals and CompareTo:");
        Console.WriteLine($"A.Equals(B): {firstMatrix.Equals(secondMatrix)}");
        Console.WriteLine($"A.CompareTo(B): {firstMatrix.CompareTo(secondMatrix)}");

        Console.WriteLine("\nChecking true / false operators:");

        if (firstMatrix) {
          Console.WriteLine("Matrix A is considered 'true' (has non-zero elements).");
        }
        else {
          Console.WriteLine("Matrix A is considered 'false' (all elements are zero).");
        }

        Console.WriteLine("\nICloneable (deep copy):");
        SquareMatrix copyMatrix = firstMatrix.DeepCopy();
        Console.WriteLine("Copy of A:");
        Console.WriteLine(copyMatrix.ToString());

        if (matrixSize > 0) {
          // Изменяем копию тестовым значением, чтобы показать глубокое копирование
          copyMatrix[0, 0] = testValueForDeepCopy;
        }

        Console.WriteLine("Original A (unchanged):");
        Console.WriteLine(firstMatrix.ToString());
        Console.WriteLine("Modified copy of A:");
        Console.WriteLine(copyMatrix.ToString());

        Console.WriteLine("\nImplicit conversion from double[,]:");
        double[,] sourceArray = { { 1, 2 }, { 3, 4 } };
        SquareMatrix matrixFromArray = sourceArray;
        Console.WriteLine(matrixFromArray.ToString());

        Console.WriteLine($"\nImplicit conversion from double ({valueForImplicitConversion}):");
        SquareMatrix matrixFromDouble = valueForImplicitConversion;
        Console.WriteLine(matrixFromDouble.ToString());
      }
      catch (MatrixException matrixException) {
        Console.WriteLine($"Matrix error: {matrixException.Message}");
      }
      catch (FormatException) {
        Console.WriteLine("Number input error.");
      }
      catch (Exception unexpectedException) {
        Console.WriteLine($"Unknown error: {unexpectedException.Message}");
      }

      Console.WriteLine("\nPress any key to exit...");
      Console.ReadKey();
    }
  }
}