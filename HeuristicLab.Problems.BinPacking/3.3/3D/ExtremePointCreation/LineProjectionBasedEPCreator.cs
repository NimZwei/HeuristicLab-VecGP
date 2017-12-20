﻿using HeuristicLab.Common;
using HeuristicLab.Problems.BinPacking3D.Geometry;
using HeuristicLab.Problems.BinPacking3D.ResidualSpaceCalculation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeuristicLab.Problems.BinPacking3D.ExtremePointCreation {
  /// <summary>
  /// This extreme point creation class uses the line projection based method for creating extreme points.
  /// This projection method enhances the point based projection method by creating extra extreme points on the intersection points of two items.
  /// </summary>
  public class LineProjectionBasedEPCreator : ExtremePointCreator {

    protected override void UpdateExtremePoints(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      binPacking.ExtremePoints.Clear();

      foreach (var i in binPacking.Items) {
        PackingItem it = i.Value;
        PackingPosition p = binPacking.Positions[i.Key];
        GenerateNewExtremePointsForItem(binPacking, it, p);
      }
    }

    /// <summary>
    /// Adds a new extreme point an the related residual spaces to a given bin packing.
    /// - The given position has to be valid.
    /// - The extreme point does not exist in the bin packing.
    /// - There must be at minimum one valid residual space. A residual space is invalid if the space is zero.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="position"></param>
    /// <returns>True = the given point and its related residual spaces were successfully added to the bin packing</returns>
    protected override bool AddExtremePoint(BinPacking3D binPacking, PackingPosition position) {
      if (position == null) {
        return false;
      }

      if (PointIsInAnyItem(binPacking, new Vector3D(position))) {
        return false;
      }

      if (binPacking.ExtremePoints.ContainsKey(position)) {
        return false;
      }

      var rs = CalculateResidualSpace(binPacking, new Vector3D(position));

      if (rs.Count() <= 0) {
        return false;
      }

      // todo 
      /*
       ist der extrempunkt im residual space eines anderen muss ueberprueft werden:
          - ist der andere ep in der luft, kann auch dieser hinzugefuegt werden.
          - hat der andere ep ein item unterhalb, darf dieser nicht hinzugefuegt werden.
       -> neu methode basierend auf IsWithinResidualSpaceOfAnotherExtremePoint, die den ep oder rs zurueck gibt, der einen anderen rs einschließt.
          eventuell gehoert diese logik in den ResidualSpaceCreator.
      if (LiesOnAnyItem(binPacking, position)) {
        return false;
      }*/

      binPacking.ExtremePoints.Add(position, rs);
      return true;
    }

    private bool LiesOnAnyItem(BinPacking3D binPacking, PackingPosition position) {
      if (position.Y == 0) {
        return true;
      }

      var items = binPacking.Items.Where(x => {
        var itemPosition = binPacking.Positions[x.Key];
        var item = x.Value;
        int width = itemPosition.Rotated ? item.Depth : item.Width;
        int depth = itemPosition.Rotated ? item.Width : item.Depth;

        return itemPosition.Y + item.Height == position.Y &&
               itemPosition.X <= position.X && position.X < itemPosition.X + width &&
               itemPosition.Z <= position.Z && position.Z < itemPosition.Z + depth;
      });

      return items.Count() > 0;
    }

    /// <summary>
    /// Getnerates the extreme points for a given item.
    /// It creates extreme points by using a point projection based method and
    /// creates points by using an edge projection based method.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="newItem"></param>
    /// <param name="position"></param>
    protected override void GenerateNewExtremePointsForItem(BinPacking3D binPacking, PackingItem newItem, PackingPosition position) {
      PointProjectionForNewItem(binPacking, newItem, position);
      EdgeProjectionForNewItem(binPacking, newItem, position);
    }

    #region Extreme point creation by using a point projection based method

    /// <summary>
    /// This method creates extreme points by using a point projection based method.
    /// For each item there will be created three points and each of the points will be projected twice.
    /// The direction of the projection depends on position of the point.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="newItem"></param>
    /// <param name="position"></param>
    private void PointProjectionForNewItem(BinPacking3D binPacking, PackingItem newItem, PackingPosition position) {
      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;
      var binShape = binPacking.BinShape;
      var sourcePoint = new PackingPosition(position.AssignedBin, position.X + newWidth, position.Y, position.Z);
      PointProjection(binPacking, sourcePoint, ProjectDown);
      PointProjection(binPacking, sourcePoint, ProjectBackward);

      sourcePoint = new PackingPosition(position.AssignedBin, position.X, position.Y + newItem.Height, position.Z);
      PointProjection(binPacking, sourcePoint, ProjectLeft);
      PointProjection(binPacking, sourcePoint, ProjectBackward);

      sourcePoint = new PackingPosition(position.AssignedBin, position.X, position.Y, position.Z + newDepth);
      PointProjection(binPacking, sourcePoint, ProjectDown);
      PointProjection(binPacking, sourcePoint, ProjectLeft);
    }


    /// <summary>
    /// Projects a given point by using the given projection method to the neares item.
    /// The given projection method returns a point which lies on a side of the nearest item in the direction. 
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="position"></param>
    /// <param name="projectionMethod"></param>
    private void PointProjection(BinPacking3D binPacking, PackingPosition position, Func<BinPacking3D, Vector3D, Vector3D> projectionMethod) {
      Vector3D sourcePoint = new Vector3D(position);
      if (sourcePoint.X < binPacking.BinShape.Width && sourcePoint.Y < binPacking.BinShape.Height && sourcePoint.Z < binPacking.BinShape.Depth) {
        Vector3D point = projectionMethod?.Invoke(binPacking, sourcePoint);
        if (point != null) {
          AddExtremePoint(binPacking, new PackingPosition(position.AssignedBin, point.X, point.Y, point.Z));
        }
      }
    }
    #endregion

    #region Extreme point creation by using an edge projection based method

    /// <summary>
    /// This method creates extreme points be projecting the edges of a given item 
    ///   - left
    ///   - back
    ///   - down.
    /// A extreme point will be created, if an edge instersects with an edge of another item.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="newItem"></param>
    /// <param name="position"></param>
    private void EdgeProjectionForNewItem(BinPacking3D binPacking, PackingItem newItem, PackingPosition position) {
      int newWidth = position.Rotated ? newItem.Depth : newItem.Width;
      int newDepth = position.Rotated ? newItem.Width : newItem.Depth;
      var binShape = binPacking.BinShape;

      foreach (var ep in GetEpsOnLeft(binPacking, newItem, position)) {
        AddExtremePoint(binPacking, ep.Key);
      }

      foreach (var ep in GetEpsBelow(binPacking, newItem, position)) {
        AddExtremePoint(binPacking, ep.Key);
      }

      foreach (var ep in GetEpsBehind(binPacking, newItem, position)) {
        AddExtremePoint(binPacking, ep.Key);
      }
    }
    #endregion

    /// <summary>
    /// Updates the residual spaces.
    /// It removes not needed ones.
    /// A residual space will be removed if the space is a subspace of another one and 
    /// the current one has no item below or both have an item below. 
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="item"></param>
    /// <param name="position"></param>
    protected override void UpdateResidualSpace(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
    }

    /// <summary>
    /// Returns true if any item in the bin packing encapsulates the given point
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private bool PointIsInAnyItem(BinPacking3D binPacking, Vector3D point) {
      foreach (var item in binPacking.Items) {
        PackingPosition position = binPacking.Positions[item.Key];
        var depth = position.Rotated ? item.Value.Width : item.Value.Depth;
        var width = position.Rotated ? item.Value.Depth : item.Value.Width;
        if (position.X <= point.X && point.X < position.X + width &&
            position.Y <= point.Y && point.Y < position.Y + item.Value.Height &&
            position.Z <= point.Z && point.Z < position.Z + depth) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Returns true if an item is in the residual space of a given extrem point
    /// </summary>
    /// <param name="rs">KeyValuePair with the position of the extreme point and the size of the residual space</param>
    /// <param name="item">Given Item</param>
    /// <param name="position">Given position</param>
    /// <returns></returns>
    private bool ItemIsInRs(KeyValuePair<PackingPosition, ResidualSpace> rs, PackingItem item, PackingPosition position) {
      return GetVertices(item, position).Where(pos => pos.IsInside(rs.Key, rs.Value)).Any();
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBelow(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key]
      }).Where(x => x.Position.Y < position.Y)
          .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsBehind(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key]
      }).Where(x => x.Position.Z < position.Z)
          .Select(x => Tuple.Create(x.Position, x.Item));
    }

    protected IEnumerable<Tuple<PackingPosition, PackingItem>> GetItemsOnLeft(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      return binPacking.Items.Select(x => new {
        Item = x.Value,
        Position = binPacking.Positions[x.Key]
      }).Where(x => x.Position.X < position.X)
          .Select(x => Tuple.Create(x.Position, x.Item));
    }

    /// <summary>
    /// Returns the extreme points and its related residual spaces on the left side of an given item.
    /// This extreme points are being created by intersecting two edges on the left side of the given item 
    /// (left - in front, left - on top) with all edges on the right side of all other items int the bin packing.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IDictionary<PackingPosition, IEnumerable<ResidualSpace>> GetEpsOnLeft(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      var eps = new SortedList<PackingPosition, IEnumerable<ResidualSpace>>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsOnLeft(binPacking, item, position);
      var edges = GetProjectionEdgesOnLeft(item, position);

      foreach (var otherItem in items) {
        if (position.Equals(otherItem.Item1)) {
          continue;
        }

        var otherItemEdges = GetEdgesOnRight(otherItem.Item2, otherItem.Item1);
        // left - in front
        foreach (var ep in IntersectionsForItem(edges[0], otherItemEdges, new Vector3D(1, 0, 0))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            // As this edge has a vertical direction, every point of intersection won't have an item below.
            // So finally it is being projected down.
            var point = ProjectDown(binPacking, ProjectLeft(binPacking, ep));
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }

        // left - on top
        foreach (var ep in IntersectionsForItem(edges[1], otherItemEdges, new Vector3D(1, 0, 0))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            var point = ProjectLeft(binPacking, ep);
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }
      }
      return eps;
    }


    /// <summary>
    /// Returns the extreme points and its related residual spaces below of an given item.
    /// This extreme points are being created by intersecting two edges below of the given item 
    /// (below - in front, below - right) with all edges on top side of all other items int the bin packing.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IDictionary<PackingPosition, IEnumerable<ResidualSpace>> GetEpsBelow(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      var eps = new SortedList<PackingPosition, IEnumerable<ResidualSpace>>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsBelow(binPacking, position);
      var edges = GetProjectionEdgesBelow(item, position);

      foreach (var otherItem in items) {
        if (position.Equals(otherItem.Item1)) {
          continue;
        }

        var otherItemEdges = GetEdgesOnTop(otherItem.Item2, otherItem.Item1);
        // below - in front
        foreach (var ep in IntersectionsForItem(edges[0], otherItemEdges, new Vector3D(0, 1, 0))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            var point = ProjectDown(binPacking, ep);
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }

        // below - right
        foreach (var ep in IntersectionsForItem(edges[1], otherItemEdges, new Vector3D(0, 1, 0))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            var point = ProjectDown(binPacking, ep);
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }
      }
      return eps;
    }

    /// <summary>
    /// Returns the extreme points and its related residual spaces below of an given item.
    /// This extreme points are being created by intersecting two edges below of the given item 
    /// (right - behind, on top - behind) with all edges on top side of all other items int the bin packing.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private IDictionary<PackingPosition, IEnumerable<ResidualSpace>> GetEpsBehind(BinPacking3D binPacking, PackingItem item, PackingPosition position) {
      var eps = new SortedList<PackingPosition, IEnumerable<ResidualSpace>>();
      IEnumerable<Tuple<PackingPosition, PackingItem>> items = GetItemsBehind(binPacking, position);
      var edges = GetProjectionEdgesBehind(item, position);

      foreach (var otherItem in items) {
        if (position.Equals(otherItem.Item1)) {
          continue;
        }

        var otherItemEdges = GetEdgesInFront(otherItem.Item2, otherItem.Item1);
        // right - behind
        foreach (var ep in IntersectionsForItem(edges[0], otherItemEdges, new Vector3D(0, 0, 1))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            // As this edge has a vertical direction, every point of intersection won't have an item below.
            // So finally it is being projected down.
            var point = ProjectDown(binPacking, ProjectBackward(binPacking, ep));
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }

        // on top - behind
        foreach (var ep in IntersectionsForItem(edges[1], otherItemEdges, new Vector3D(0, 0, 1))) {
          if (ep.X < binPacking.BinShape.Width && ep.Y < binPacking.BinShape.Height && ep.Z < binPacking.BinShape.Depth) {
            var point = ProjectBackward(binPacking, ep);
            var residualSpaces = CalculateResidualSpace(binPacking, point);
            var newExtremePoint = point.ToPackingPosition(position.AssignedBin);
            if (residualSpaces.Count() > 0 && !eps.ContainsKey(newExtremePoint)) {
              eps.Add(newExtremePoint, residualSpaces);
            }
          }
        }
      }
      return eps;
    }

    #region Methods for getting the edges for items

    /// <summary>
    /// Returns an array of packing position which represents the vertices of an item.
    /// The position of a vertex in the array is mapped to an item as followed:
    ///      4----------5
    ///     /|         /|
    ///    / |        / |
    ///   /  0-------/--1
    ///  6--/-------7  /
    ///  | /        | /
    ///  |/         |/
    ///  2----------3
    ///  
    ///  0 = (0,0,0)
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Vector3D[] GetVertices(PackingItem item, PackingPosition position) {
      int width = position.Rotated ? item.Depth : item.Width;
      int depth = position.Rotated ? item.Width : item.Depth;
      return new Vector3D[] {
        new Vector3D(position.X + 0,     position.Y + 0,           position.Z + 0), // (0,0,0)
        new Vector3D(position.X + width, position.Y + 0,           position.Z + 0), // (x,0,0)
        new Vector3D(position.X + 0,     position.Y + 0,           position.Z + depth), // (0,0,z)
        new Vector3D(position.X + width, position.Y + 0,           position.Z + depth), // (x,0,z)

        new Vector3D(position.X + 0,     position.Y + item.Height, position.Z + 0), // (0,y,0)
        new Vector3D(position.X + width, position.Y + item.Height, position.Z + 0), // (x,y,0)
        new Vector3D(position.X + 0,     position.Y + item.Height, position.Z + depth), // (0,y,z)
        new Vector3D(position.X + width, position.Y + item.Height, position.Z + depth), //(x,y,z)
      };
    }

    private Edge3D[] GetProjectionEdgesOnLeft(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[6]),
        new Edge3D(points[6], points[4])
      };
    }

    private Edge3D[] GetProjectionEdgesBelow(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[3]),
        new Edge3D(points[3], points[1])
      };
    }

    private Edge3D[] GetProjectionEdgesBehind(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[1], points[5]),
        new Edge3D(points[5], points[4])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges of the rigth side of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesOnRight(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[1], points[5]),
        new Edge3D(points[5], points[7]),
        new Edge3D(points[7], points[3]),
        new Edge3D(points[3], points[1])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges on the top of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesOnTop(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[4], points[5]),
        new Edge3D(points[5], points[7]),
        new Edge3D(points[7], points[6]),
        new Edge3D(points[6], points[4])
      };
    }

    /// <summary>
    /// Returns an array of edges which contains all edges in front of an given item.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="position"></param>
    /// <returns></returns>
    private Edge3D[] GetEdgesInFront(PackingItem item, PackingPosition position) {
      Vector3D[] points = GetVertices(item, position);

      return new Edge3D[] {
        new Edge3D(points[2], points[3]),
        new Edge3D(points[3], points[7]),
        new Edge3D(points[7], points[6]),
        new Edge3D(points[6], points[2])
      };
    }

    #endregion


    #region Intersections

    /// <summary>
    /// Returns a collection of points where a given edge (projectedEdge) intersects with other edges.
    /// The given edge (projectedEdge) will be projected by using the given vector direction
    /// and a edge of the given edge collection.
    /// The returned collecten can be empty.
    /// </summary>
    /// <param name="projectedEdge"></param>
    /// <param name="edges"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    private IEnumerable<Vector3D> IntersectionsForItem(Edge3D projectedEdge, Edge3D[] edges, Vector3D direction = null) {
      IList<Vector3D> eps = new List<Vector3D>();
      foreach (var edge in edges) {
        Edge3D e = projectedEdge;
        if (direction != null) {
          if (direction.X != 0) {
            e.Start.X = edge.Start.X;
            e.End.X = edge.End.X;
          } else if (direction.Y != 0) {
            e.Start.Y = edge.Start.Y;
            e.End.Y = edge.End.Y;
          } else if (direction.Z != 0) {
            e.Start.Z = edge.Start.Z;
            e.End.Z = edge.End.Z;
          }
        }

        var ep = edge.Intersects(e);
        if (ep != null) {
          eps.Add(ep);
        }
      }
      return eps as IEnumerable<Vector3D>;
    }



    #endregion

    /// <summary>
    /// Calculates the residual spaces for an extreme point.
    /// </summary>
    /// <param name="binPacking"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    protected override IEnumerable<ResidualSpace> CalculateResidualSpace(BinPacking3D binPacking, Vector3D pos) {
      return ResidualSpaceCalculatorFactory.CreateCalculator().CalculateResidualSpaces(binPacking, pos);
    }
  }


}
