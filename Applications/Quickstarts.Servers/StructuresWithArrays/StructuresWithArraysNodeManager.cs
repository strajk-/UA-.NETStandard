/* ========================================================================
 * Copyright (c) 2005-2019 The OPC Foundation, Inc. All rights reserved.
 *
 * OPC Foundation MIT License 1.00
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 *
 * The complete license agreement can be found here:
 * http://opcfoundation.org/License/MIT/1.00/
 * ======================================================================*/

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Threading;
using Opc.Ua;
using Opc.Ua.Server;
using System.Reflection;
using System.Linq;

namespace StructuresWithArrays
{
    /// <summary>
    /// The node manager factory for test data.
    /// </summary>
    public class StructuresWithArraysManagerFactory : INodeManagerFactory
    {
        /// <inheritdoc/>
        public INodeManager Create(IServerInternal server, ApplicationConfiguration configuration)
        {
            return new StructuresWithArraysNodeManager(server, configuration, NamespacesUris.ToArray());
        }

        /// <inheritdoc/>
        public StringCollection NamespacesUris
        {
            get
            {
                var nameSpaces = new StringCollection {
                    Namespaces.StructuresWithArrays
                };
                return nameSpaces;
            }
        }
    }

    /// <summary>
    /// A node manager for a variety of test data.
    /// </summary>
    public class StructuresWithArraysNodeManager : CustomNodeManager2
    {
        #region Constructors
        /// <summary>
        /// Initializes the node manager.
        /// </summary>
        public StructuresWithArraysNodeManager(Opc.Ua.Server.IServerInternal server, ApplicationConfiguration configuration, string[] namespaceUris)
        :
            base(server, configuration)
        {
            // update the namespaces.
            NamespaceUris = namespaceUris;

            Server.Factory.AddEncodeableTypes(typeof(StructuresWithArraysNodeManager).Assembly.GetExportedTypes().Where(t => t.FullName.StartsWith(typeof(StructuresWithArraysNodeManager).Namespace)));

            // get the configuration for the node manager.
            m_configuration = configuration.ParseExtension<StructuresWithArraysConfiguration>();

            // use suitable defaults if no configuration exists.
            if (m_configuration == null)
            {
                m_configuration = new StructuresWithArraysConfiguration();
            }

            // create the object used to access the test system.
            m_system = new StructuresWithArraysSystem(this, server.NamespaceUris, server.ServerUris);

            // update the default context.
            SystemContext.SystemHandle = m_system;
        }
        #endregion

        #region INodeIdFactory Members
        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="node">The node.</param>
        /// <returns>The new NodeId.</returns>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            uint id = Utils.IncrementIdentifier(ref m_lastUsedId);
            return new NodeId(id, m_namespaceIndex);
        }
        #endregion

        #region INodeManager Members
        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                // ensure the namespace used by the node manager is in the server's namespace table.
                m_namespaceIndex = Server.NamespaceUris.GetIndexOrAppend(Namespaces.StructuresWithArrays);

                base.CreateAddressSpace(externalReferences);

                CreateStructures(SystemContext);
            }
        }

        /// <summary>
        /// Loads a node set from a file or resource and addes them to the set of predefined nodes.
        /// </summary>
        protected override NodeStateCollection LoadPredefinedNodes(ISystemContext context)
        {
            NodeStateCollection predefinedNodes = new NodeStateCollection();
            predefinedNodes.LoadFromBinaryResource(context, "Quickstarts.Servers.StructuresWithArrays.StructuresWithArrays.PredefinedNodes.uanodes",
                this.GetType().GetTypeInfo().Assembly, true);
            return predefinedNodes;
        }

        /// <summary>
        /// Replaces the generic node with a node specific to the model.
        /// </summary>
        protected override NodeState AddBehaviourToPredefinedNode(ISystemContext context, NodeState predefinedNode)
        {
            var variableNode = predefinedNode as BaseVariableState;
            if (variableNode != null)
            {
                if (variableNode.Value == null)
                {
                    var dataTypeId = variableNode.DataType;
                    if (IsNodeIdInNamespace(variableNode.DataType))
                    {
                        if (variableNode.Parent is BaseVariableTypeState)
                        {
                            return predefinedNode;
                        }

                        switch ((uint)dataTypeId.Identifier)
                        {
                            case DataTypes.Structure_A:
                            {
                                if (variableNode is Structure_A_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_A_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }

                                return activeNode;
                            }
                            case DataTypes.Structure_B:
                            {
                                if (variableNode is Structure_B_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_B_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_C:
                            {
                                if (variableNode is Structure_C_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_C_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_D:
                            {
                                if (variableNode is Structure_D_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_D_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_E:
                            {
                                if (variableNode is Structure_E_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_E_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_F:
                            {
                                if (variableNode is Structure_F_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_F_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_G:
                            {
                                if (variableNode is Structure_G_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_G_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_H:
                            {
                                if (variableNode is Structure_H_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_H_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_J:
                            {
                                if (variableNode is Structure_J_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_J_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_K:
                            {
                                if (variableNode is Structure_K_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_K_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_L:
                            {
                                if (variableNode is Structure_L_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_L_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_M:
                            {
                                if (variableNode is Structure_M_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_M_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_N:
                            {
                                if (variableNode is Structure_N_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_N_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_O:
                            {
                                if (variableNode is Structure_O_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_O_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_P:
                            {
                                if (variableNode is Structure_P_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_P_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_Q:
                            {
                                if (variableNode is Structure_Q_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_Q_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_R:
                            {
                                if (variableNode is Structure_R__TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_R__TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_S:
                            {
                                if (variableNode is Structure_S_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_S_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.Structure_T:
                            {
                                if (variableNode is Structure_T_TypeState)
                                {
                                    break;
                                }

                                var activeNode = new Structure_T_TypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }
                            case DataTypes.LargeComplexStructure:
                            {
                                if (variableNode is LargeComplexStructureTypeState)
                                {
                                    break;
                                }

                                var activeNode = new LargeComplexStructureTypeState(variableNode.Parent);
                                activeNode.Create(context, variableNode);

                                if (variableNode.Parent != null)
                                {
                                    variableNode.Parent.ReplaceChild(context, activeNode);
                                }
                                return activeNode;
                            }

                            default: break;
                        }
                    }
                }
            }

            var passiveNode = predefinedNode as BaseObjectState;

            if (passiveNode == null)
            {
                return predefinedNode;
            }

            NodeId typeId = passiveNode.TypeDefinitionId;

            if (!IsNodeIdInNamespace(typeId) || typeId.IdType != IdType.Numeric)
            {
                return predefinedNode;
            }

            return predefinedNode;
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateStructures(ServerSystemContext systemContext)
        {
            {
                const uint Structures_LargeArray = 6156;
                LargeComplexStructureTypeState variable = FindTypeState<LargeComplexStructureTypeState>(Structures_LargeArray);
                m_largeArray = new LargeComplexStructureTypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_A = 6204;
                Structure_A_TypeState variable = FindTypeState<Structure_A_TypeState>(Structures_Structure_A);
                m_scalarStructureA = new Structure_A_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_B = 6008;
                Structure_B_TypeState variable = FindTypeState<Structure_B_TypeState>(Structures_Structure_B);
                m_scalarStructureB = new Structure_B_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_C = 6010;
                Structure_C_TypeState variable = FindTypeState<Structure_C_TypeState>(Structures_Structure_C);
                m_scalarStructureC = new Structure_C_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_D = 6112;
                Structure_D_TypeState variable = FindTypeState<Structure_D_TypeState>(Structures_Structure_D);
                m_scalarStructureD = new Structure_D_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_E = 6114;
                Structure_E_TypeState variable = FindTypeState<Structure_E_TypeState>(Structures_Structure_E);
                m_scalarStructureE = new Structure_E_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_F = 6117;
                Structure_F_TypeState variable = FindTypeState<Structure_F_TypeState>(Structures_Structure_F);
                m_scalarStructureF = new Structure_F_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_G = 6126;
                Structure_G_TypeState variable = FindTypeState<Structure_G_TypeState>(Structures_Structure_G);
                m_scalarStructureG = new Structure_G_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_H = 6130;
                Structure_H_TypeState variable = FindTypeState<Structure_H_TypeState>(Structures_Structure_H);
                m_scalarStructureH = new Structure_H_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_J = 6132;
                Structure_J_TypeState variable = FindTypeState<Structure_J_TypeState>(Structures_Structure_J);
                m_scalarStructureJ = new Structure_J_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_K = 6134;
                Structure_K_TypeState variable = FindTypeState<Structure_K_TypeState>(Structures_Structure_K);
                m_scalarStructureK = new Structure_K_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_L = 6136;
                Structure_L_TypeState variable = FindTypeState<Structure_L_TypeState>(Structures_Structure_L);
                m_scalarStructureL = new Structure_L_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_M = 6138;
                Structure_M_TypeState variable = FindTypeState<Structure_M_TypeState>(Structures_Structure_M);
                m_scalarStructureM = new Structure_M_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_N = 6140;
                Structure_N_TypeState variable = FindTypeState<Structure_N_TypeState>(Structures_Structure_N);
                m_scalarStructureN = new Structure_N_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_O = 6142;
                Structure_O_TypeState variable = FindTypeState<Structure_O_TypeState>(Structures_Structure_O);
                m_scalarStructureO = new Structure_O_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_P = 6144;
                Structure_P_TypeState variable = FindTypeState<Structure_P_TypeState>(Structures_Structure_P);
                m_scalarStructureP = new Structure_P_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_Q = 6147;
                Structure_Q_TypeState variable = FindTypeState<Structure_Q_TypeState>(Structures_Structure_Q);
                m_scalarStructureQ = new Structure_Q_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_R = 6149;
                Structure_R__TypeState variable = FindTypeState<Structure_R__TypeState>(Structures_Structure_R);
                m_scalarStructureR = new Structure_R__TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_S = 6151;
                Structure_S_TypeState variable = FindTypeState<Structure_S_TypeState>(Structures_Structure_S);
                m_scalarStructureS = new Structure_S_TypeValue(variable, null, m_lock);
            }
            {
                const uint Structures_Structure_T = 6153;
                Structure_T_TypeState variable = FindTypeState<Structure_T_TypeState>(Structures_Structure_T);
                m_scalarStructureT = new Structure_T_TypeValue(variable, null, m_lock);
            }
        }

        private TS FindTypeState<TS>(uint nodeId)
            where TS : NodeState
        {
            var expandedNodeId = new ExpandedNodeId(nodeId, Namespaces.StructuresWithArrays);
            return FindPredefinedNode(
                ExpandedNodeId.ToNodeId(expandedNodeId, Server.NamespaceUris),
                typeof(TS)) as TS;
        }
        #endregion

        #region Private Fields
        private object m_lock = new object();
        private StructuresWithArraysConfiguration m_configuration;
        private ushort m_namespaceIndex;
        private StructuresWithArraysSystem m_system;
        private long m_lastUsedId;
        // Variables
        private LargeComplexStructureTypeValue m_largeArray;
        private Structure_A_TypeValue m_scalarStructureA;
        private Structure_B_TypeValue m_scalarStructureB;
        private Structure_C_TypeValue m_scalarStructureC;
        private Structure_D_TypeValue m_scalarStructureD;
        private Structure_E_TypeValue m_scalarStructureE;
        private Structure_F_TypeValue m_scalarStructureF;
        private Structure_G_TypeValue m_scalarStructureG;
        private Structure_H_TypeValue m_scalarStructureH;
        private Structure_J_TypeValue m_scalarStructureJ;
        private Structure_K_TypeValue m_scalarStructureK;
        private Structure_L_TypeValue m_scalarStructureL;
        private Structure_M_TypeValue m_scalarStructureM;
        private Structure_N_TypeValue m_scalarStructureN;
        private Structure_O_TypeValue m_scalarStructureO;
        private Structure_P_TypeValue m_scalarStructureP;
        private Structure_Q_TypeValue m_scalarStructureQ;
        private Structure_R__TypeValue m_scalarStructureR;
        private Structure_S_TypeValue m_scalarStructureS;
        private Structure_T_TypeValue m_scalarStructureT;
        #endregion
    }
}
