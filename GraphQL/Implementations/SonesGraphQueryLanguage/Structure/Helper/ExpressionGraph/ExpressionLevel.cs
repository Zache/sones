/*
* sones GraphDB - Community Edition - http://www.sones.com
* Copyright (C) 2007-2011 sones GmbH
*
* This file is part of sones GraphDB Community Edition.
*
* sones GraphDB is free software: you can redistribute it and/or modify
* it under the terms of the GNU Affero General Public License as published by
* the Free Software Foundation, version 3 of the License.
* 
* sones GraphDB is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
* GNU Affero General Public License for more details.
*
* You should have received a copy of the GNU Affero General Public License
* along with sones GraphDB. If not, see <http://www.gnu.org/licenses/>.
* 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using sones.GraphQL.GQL.Structure.Helper.ExpressionGraph.Helper;
using sones.Library.PropertyHyperGraph;
using sones.GraphQL.GQL.ErrorHandling;
using sones.Library.Commons.VertexStore.Definitions;

namespace sones.GraphQL.GQL.Structure.Helper.ExpressionGraph
{
    /// <summary>
    /// This class implements the levels of the expression graph.
    /// </summary>
    public sealed class ExpressionLevel : IExpressionLevel
    {
        #region Properties

        /// <summary>
        /// The content of the level
        /// </summary>
        private Dictionary<LevelKey, IExpressionLevelEntry> _Content;

        public Dictionary<LevelKey, IExpressionLevelEntry> ExpressionLevels
        {
            get
            {
                lock (_Content)
                {
                    return _Content;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ExpressionLevel()
        {
            _Content = new Dictionary<LevelKey, IExpressionLevelEntry>();
        }

        #endregion

        #region IExpressionLevel Members

        public void AddNodeAndBackwardEdge(LevelKey myPath, IVertex aVertex, EdgeKey backwardDirection, VertexInformation backwardDestination, IComparable EdgeWeight, IComparable NodeWeight)
        {
            lock (_Content)
            {
                var node = GenerateVertexInfoFromLevelKeyAndVertexID(aVertex.VertexTypeID, aVertex.VertexID);

                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(node))
                    {


                        //Node exists
                        _Content[myPath].Nodes[node].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);


                    }
                    else
                    {

                        //Node does not exist
                        _Content[myPath].Nodes.Add(node, new ExpressionNode(aVertex, NodeWeight, node));
                        _Content[myPath].Nodes[node].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                    }
                }
                else
                {
                    HashSet<IExpressionEdge> backwardEdges = new HashSet<IExpressionEdge>() { new ExpressionEdge(backwardDestination, EdgeWeight, backwardDirection) };

                    _Content.Add(myPath, new ExpressionLevelEntry(myPath));
                    _Content[myPath].Nodes.Add(node, new ExpressionNode(aVertex, NodeWeight, node));
                    _Content[myPath].Nodes[node].AddBackwardEdge(backwardDirection, backwardDestination, EdgeWeight);

                }

            }
        }

        public void AddBackwardEdgesToNode(LevelKey myPath, VertexInformation myVertexInformation, EdgeKey backwardDestination, Dictionary<VertexInformation, IComparable> validUUIDs)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myPath))
                {
                    //the level exists
                    if (_Content[myPath].Nodes.ContainsKey(myVertexInformation))
                    {

                        //Node exists
                        _Content[myPath].Nodes[myVertexInformation].AddBackwardEdges(backwardDestination, validUUIDs);

                    }
                    else
                    {
                        //Node does not exist
                        throw new ExpressionGraphInternalException("The node does not exist in this LevelKey.");
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new ExpressionGraphInternalException("The LevelKey does not exist in this ExpressionLevel.");
                }
            }
        }

        private VertexInformation GenerateVertexInfoFromLevelKeyAndVertexID(long myVertexTypeName, long myVertexID)
        {
            return new VertexInformation(myVertexTypeName, myVertexID);
        }

        public void AddForwardEdgeToNode(LevelKey levelKey, VertexInformation myVertexInformation, EdgeKey forwardDirection, VertexInformation destination, IComparable edgeWeight)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    //the level exists
                    if (_Content[levelKey].Nodes.ContainsKey(myVertexInformation))
                    {
                        //Node exists
                        _Content[levelKey].Nodes[myVertexInformation].AddForwardEdge(forwardDirection, destination, edgeWeight);
                    }
                    else
                    {
                        //Node does not exist
                        throw new ExpressionGraphInternalException("The node does not exist in this LevelKey.");
                    }
                }
                else
                {
                    //LevelKey does not exist
                    throw new ExpressionGraphInternalException("The LevelKey does not exist in this ExpressionLevel.");
                }
            }
        }

        public void AddNode(LevelKey levelKey, IExpressionNode expressionNode)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(levelKey))
                {
                    var tempInt64 = expressionNode.GetIVertex().VertexID;

                    var node = expressionNode.VertexInformation;

                    if (_Content[levelKey].Nodes.ContainsKey(node))
                    {
                        //the node already exist, so update its edges
                        foreach (var aBackwardEdge in expressionNode.BackwardEdges)
                        {
                            _Content[levelKey].Nodes[node].AddBackwardEdges(aBackwardEdge.Value);
                        }

                        foreach (var aForwardsEdge in expressionNode.ForwardEdges)
                        {
                            _Content[levelKey].Nodes[node].AddForwardEdges(aForwardsEdge.Value);
                        }
                    }
                    else
                    {
                        _Content[levelKey].Nodes.Add(node, expressionNode);
                    }
                }
                else
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));

                    _Content[levelKey].Nodes.Add(expressionNode.VertexInformation, expressionNode);
                }
            }
        }

        public void AddEmptyLevelKey(LevelKey levelKey)
        {
            lock (_Content)
            {
                if (!_Content.ContainsKey(levelKey))
                {
                    _Content.Add(levelKey, new ExpressionLevelEntry(levelKey));
                }
            }
        }

        public IEnumerable<IExpressionNode> GetNodes(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                if (_Content.ContainsKey(myLevelKey))
                {
                    return _Content[myLevelKey].Nodes.Values;
                }

                return null;
            }
        }

        public Dictionary<VertexInformation, IExpressionNode> GetNode(LevelKey myLevelKey)
        {
            lock (_Content)
            {

                if (_Content.ContainsKey(myLevelKey))
                    return _Content[myLevelKey].Nodes;

                return null;
            }
        }

        public void RemoveNode(LevelKey myLevelKey, VertexInformation myToBeRemovedVertex)
        {
            lock (_Content)
            {

                if (_Content.ContainsKey(myLevelKey))
                {
                    _Content[myLevelKey].Nodes.Remove(myToBeRemovedVertex);
                }
            }
        }

        public Boolean HasLevelKey(LevelKey myLevelKey)
        {
            lock (_Content)
            {
                return _Content.ContainsKey(myLevelKey);
            }
        }

        #endregion

        #region override

        public override string ToString()
        {
            return String.Format("Level#: {0}", ExpressionLevels.Count);
        }

        #endregion
    }
}
