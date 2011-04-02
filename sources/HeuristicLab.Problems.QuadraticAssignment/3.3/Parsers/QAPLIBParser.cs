﻿#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2011 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
 *
 * This file is part of HeuristicLab.
 *
 * HeuristicLab is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * HeuristicLab is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with HeuristicLab. If not, see <http://www.gnu.org/licenses/>.
 */
#endregion

using System;
using System.IO;

namespace HeuristicLab.Problems.QuadraticAssignment {
  public class QAPLIBParser {
    public int Size { get; private set; }
    public double[,] Distances { get; private set; }
    public double[,] Weights { get; private set; }
    public Exception Error { get; private set; }

    public QAPLIBParser() {
      Reset();
    }

    public void Reset() {
      Size = 0;
      Distances = null;
      Weights = null;
      Error = null;
    }

    public bool Parse(string file) {
      using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
        return Parse(stream);
      }
    }

    /// <summary>
    /// Reads from the given stream data which is expected to be in the QAPLIB format.
    /// </summary>
    /// <remarks>
    /// The stream is not closed or disposed. The caller has to take care of that.
    /// </remarks>
    /// <param name="stream">The stream to read data from.</param>
    /// <returns>True if the file was successfully read or false otherwise.</returns>
    public bool Parse(Stream stream) {
      Error = null;
      try {
        StreamReader reader = new StreamReader(stream);
        Size = int.Parse(reader.ReadLine());
        Distances = new double[Size, Size];
        Weights = new double[Size, Size];
        string valLine = reader.ReadLine();
        char[] delim = new char[] { ' ' };
        for (int i = 0; i < Size; i++) {
          if (i > 0 || String.IsNullOrWhiteSpace(valLine))
            valLine = reader.ReadLine();
          string[] vals = new string[Size];
          string[] partVals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          partVals.CopyTo(vals, 0);
          int index = partVals.Length;
          while (index < Size) {
            valLine = reader.ReadLine();
            partVals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
            partVals.CopyTo(vals, index);
            index += partVals.Length;
          }
          for (int j = 0; j < Size; j++) {
            Distances[i, j] = double.Parse(vals[j]);
          }
        }
        valLine = reader.ReadLine();
        int read = 0;
        int k = 0;
        while (!reader.EndOfStream) {
          if (read > 0 || String.IsNullOrWhiteSpace(valLine))
            valLine = reader.ReadLine();
          string[] vals = valLine.Split(delim, StringSplitOptions.RemoveEmptyEntries);
          for (int j = 0; j < vals.Length; j++) {
            if (read + j == Size) {
              read = 0;
              k++;
            }
            Weights[k, read + j] = double.Parse(vals[j]);
          }
          read += vals.Length;
        }
        return true;
      } catch (Exception e) {
        Error = e;
        return false;
      }
    }
  }
}
