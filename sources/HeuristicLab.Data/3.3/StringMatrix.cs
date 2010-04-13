#region License Information
/* HeuristicLab
 * Copyright (C) 2002-2010 Heuristic and Evolutionary Algorithms Laboratory (HEAL)
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
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using HeuristicLab.Common;
using HeuristicLab.Core;
using HeuristicLab.Persistence.Default.CompositeSerializers.Storable;

namespace HeuristicLab.Data {
  [Item("StringMatrix", "Represents a matrix of strings.")]
  [StorableClass]
  public class StringMatrix : Item, IEnumerable, IStringConvertibleMatrix {
    public override Image ItemImage {
      get { return HeuristicLab.Common.Resources.VS2008ImageLibrary.Class; }
    }

    [Storable]
    protected string[,] matrix;

    [Storable]
    private List<string> columnNames;
    public IEnumerable<string> ColumnNames {
      get { return this.columnNames; }
      set {
        if (value == null || value.Count() == 0)
          columnNames = new List<string>();
        else if (value.Count() != Columns)
          throw new ArgumentException("A column name must be specified for each column .");
        else
          columnNames = new List<string>(value);
      }
    }
    [Storable]
    private List<string> rowNames;
    public IEnumerable<string> RowNames {
      get { return this.rowNames; }
      set {
        if (value == null || value.Count() == 0)
          rowNames = new List<string>();
        else if (value.Count() != Rows)
          throw new ArgumentException("A row name must be specified for each row.");
        else
          rowNames = new List<string>(value);
      }
    }
    [Storable]
    private bool sortableView;
    public bool SortableView {
      get { return sortableView; }
      set {
        if (value != sortableView) {
          sortableView = value;
          OnSortableViewChanged();
        }
      }
    }

    public virtual int Rows {
      get { return matrix.GetLength(0); }
      protected set {
        if (value != Rows) {
          string[,] newMatrix = new string[value, Columns];
          Array.Copy(matrix, newMatrix, Math.Min(value * Columns, matrix.Length));
          matrix = newMatrix;
          while (rowNames.Count > value)
            rowNames.RemoveAt(rowNames.Count - 1);
          while (rowNames.Count < value)
            rowNames.Add("Row " + rowNames.Count);
          OnReset();
        }
      }
    }
    public virtual int Columns {
      get { return matrix.GetLength(1); }
      protected set {
        if (value != Columns) {
          string[,] newMatrix = new string[Rows, value];
          for (int i = 0; i < Rows; i++)
            Array.Copy(matrix, i * Columns, newMatrix, i * value, Math.Min(value, Columns));
          matrix = newMatrix;
          while (columnNames.Count > value)
            columnNames.RemoveAt(columnNames.Count - 1);
          while (columnNames.Count < value)
            columnNames.Add("Column " + columnNames.Count);
          OnReset();
        }
      }
    }
    public virtual string this[int rowIndex, int columnIndex] {
      get { return matrix[rowIndex, columnIndex]; }
      set {
        if (value != matrix[rowIndex, columnIndex]) {
          if ((value != null) || (matrix[rowIndex, columnIndex] != string.Empty)) {
            matrix[rowIndex, columnIndex] = value != null ? value : string.Empty;
            OnItemChanged(rowIndex, columnIndex);
          }
        }
      }
    }

    public StringMatrix() {
      matrix = new string[0, 0];
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
    }
    public StringMatrix(int rows, int columns) {
      matrix = new string[rows, columns];
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++)
          matrix[i, j] = string.Empty;
      }
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
    }
    protected StringMatrix(int rows, int columns, IEnumerable<string> columnNames)
      : this(rows, columns) {
      ColumnNames = columnNames;
    }
    protected StringMatrix(int rows, int columns, IEnumerable<string> columnNames, IEnumerable<string> rowNames)
      : this(rows, columns,columnNames) {
      RowNames = rowNames;
    }
    public StringMatrix(string[,] elements) {
      if (elements == null) throw new ArgumentNullException();
      matrix = new string[elements.GetLength(0), elements.GetLength(1)];
      for (int i = 0; i < matrix.GetLength(0); i++) {
        for (int j = 0; j < matrix.GetLength(1); j++)
          matrix[i, j] = elements[i, j] == null ? string.Empty : elements[i, j];
      }
      columnNames = new List<string>();
      rowNames = new List<string>();
      sortableView = false;
    }
    protected StringMatrix(string[,] elements, IEnumerable<string> columnNames)
      : this(elements) {
      ColumnNames = columnNames;
    }
    protected StringMatrix(string[,] elements, IEnumerable<string> columnNames,IEnumerable<string> rowNames)
      : this(elements,columnNames) {
      RowNames = rowNames;
    }

    public override IDeepCloneable Clone(Cloner cloner) {
      StringMatrix clone = new StringMatrix();
      cloner.RegisterClonedObject(this, clone);
      clone.ReadOnlyView = ReadOnlyView;
      clone.SortableView = SortableView;
      clone.matrix = (string[,])matrix.Clone();
      clone.columnNames = new List<string>(columnNames);
      clone.rowNames = new List<string>(rowNames);
      return clone;
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      if (matrix.Length > 0) {
        for (int i = 0; i < Rows; i++) {
          sb.Append("[").Append(matrix[i, 0]);
          for (int j = 1; j < Columns; j++)
            sb.Append(";").Append(matrix[i, j]);
          sb.Append("]");
        }
      }
      sb.Append("]");
      return sb.ToString();
    }

    public virtual IEnumerator GetEnumerator() {
      return matrix.GetEnumerator();
    }

    protected virtual bool Validate(string value, out string errorMessage) {
      if (value == null) {
        errorMessage = "Invalid Value (string must not be null)";
        return false;
      } else {
        errorMessage = string.Empty;
        return true;
      }
    }
    protected virtual string GetValue(int rowIndex, int columIndex) {
      return this[rowIndex, columIndex];
    }
    protected virtual bool SetValue(string value, int rowIndex, int columnIndex) {
      if (value != null) {
        this[rowIndex, columnIndex] = value;
        return true;
      } else {
        return false;
      }
    }

    public event EventHandler ColumnNamesChanged;
    protected virtual void OnColumnNamesChanged() {
      EventHandler handler = ColumnNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler RowNamesChanged;
    protected virtual void OnRowNamesChanged() {
      EventHandler handler = RowNamesChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler SortableViewChanged;
    protected virtual void OnSortableViewChanged() {
      EventHandler handler = SortableViewChanged;
      if (handler != null)
        handler(this, EventArgs.Empty);
    }
    public event EventHandler<EventArgs<int, int>> ItemChanged;
    protected virtual void OnItemChanged(int rowIndex, int columnIndex) {
      if (ItemChanged != null)
        ItemChanged(this, new EventArgs<int, int>(rowIndex, columnIndex));
      OnToStringChanged();
    }
    public event EventHandler Reset;
    protected virtual void OnReset() {
      if (Reset != null)
        Reset(this, EventArgs.Empty);
      OnToStringChanged();
    }

    #region IStringConvertibleMatrix Members
    int IStringConvertibleMatrix.Rows {
      get { return Rows; }
      set { Rows = value; }
    }
    int IStringConvertibleMatrix.Columns {
      get { return Columns; }
      set { Columns = value; }
    }
    IEnumerable<string> IStringConvertibleMatrix.ColumnNames {
      get { return this.ColumnNames; }
      set { this.ColumnNames = value; }
    }
    IEnumerable<string> IStringConvertibleMatrix.RowNames {
      get { return this.RowNames; }
      set { this.RowNames = value; }
    }
    bool IStringConvertibleMatrix.SortableView {
      get { return this.SortableView; }
      set { this.SortableView = value; }
    }
    bool IStringConvertibleMatrix.Validate(string value, out string errorMessage) {
      return Validate(value, out errorMessage);
    }
    string IStringConvertibleMatrix.GetValue(int rowIndex, int columIndex) {
      return GetValue(rowIndex, columIndex);
    }
    bool IStringConvertibleMatrix.SetValue(string value, int rowIndex, int columnIndex) {
      return SetValue(value, rowIndex, columnIndex);
    }
    #endregion
  }
}
