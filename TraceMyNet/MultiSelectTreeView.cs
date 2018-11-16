#if LATER

namespace MultiSelectTree
{
    using System;
    using System.Collections;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    //////////////////////////////////////////////////////////////////////////
    /// Events
    //////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////
    /// CMSTreeView controls throws events related to the selected nodes 
    /// in the following order:
    ///		OnBeforeMultiSelectStart  - being fired when multiselection is started but before any node changes state
    ///		OnBeforeSelect/OnBeforeUnselect - being fired for every node in the multiselection group, before node changes state
    ///		OnAfterSelect/OnAfterUnselect - being fired for every node in the multiselection group, after node changed state
    ///		...
    ///		OnAfterMultiSelectComplete  - being fired when multiselection is finished
    ///	
    ///	Notes:
    ///		- user can cancel node selection by setting Cancel property on OnBeforeSelect event
    ///		- when user collapses a node, selection from children elements are being dropped.
    ///		All OnBeforeSelect/OnAfterSelect and OnAfterMultiSelectComplete are being fired as usually.
    //////////////////////////////////////////////////////////////////////////


    //////////////////////////////////////////////////////////////////////////////////
    /// CMSTreeView
    /// //////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Summary description for CMSTreeView.
    /// </summary>
    public class CMSTreeView : TreeView
    {
        protected ArrayList selectedNodes = null; // list of nodes in SORTED order

        private bool allowEmptySelection = true;
        private bool allowMultiSelection = true;
        private bool bEnableSelectEvents = true;

        private TreeViewCancelEventHandler onBeforeSelect;
        private TreeViewEventHandler onAfterSelect;
        private TreeViewCancelEventHandler onBeforeUnselect;
        private TreeViewEventHandler onAfterUnselect;
        private CMSTreeViewMultiSelectCompleteEventHandler onAfterMultiSelectComplete;
        private CMSTreeViewMultiSelectStartEventHandler onBeforeMultiSelectStart;

#if MSTREE_TRACE
		private int traceCount = 0;
#endif

        public CMSTreeView()
        {
            this.selectedNodes = new ArrayList();
        }

        // events
        public new event TreeViewCancelEventHandler BeforeSelect
        {
            add
            {
                onBeforeSelect += value;
            }
            remove
            {
                onBeforeSelect -= value;
            }
        }


        public new event TreeViewEventHandler AfterSelect
        {
            add
            {
                onAfterSelect += value;
            }
            remove
            {
                onAfterSelect -= value;
            }
        }

        public event TreeViewCancelEventHandler BeforeUnselect
        {
            add
            {
                onBeforeUnselect += value;
            }
            remove
            {
                onBeforeUnselect -= value;
            }
        }


        public event TreeViewEventHandler AfterUnselect
        {
            add
            {
                onAfterUnselect += value;
            }
            remove
            {
                onAfterUnselect -= value;
            }
        }

        public event CMSTreeViewMultiSelectCompleteEventHandler AfterMultiSelectComplete
        {
            add
            {
                onAfterMultiSelectComplete += value;
            }
            remove
            {
                onAfterMultiSelectComplete -= value;
            }
        }

        public event CMSTreeViewMultiSelectStartEventHandler BeforeMultiSelectStart
        {
            add
            {
                onBeforeMultiSelectStart += value;
            }
            remove
            {
                onBeforeMultiSelectStart -= value;
            }
        }


        // hide TreeView OnBeforeSelect and OnAfterSelect methods
        [Obsolete("Use OnBeforeSelect(CMSTreeViewCancelEventArgs e) instead", false)]
        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            base.OnBeforeSelect(e);
        }

        [Obsolete("Use OnAfterSelect(CMSTreeViewEventArgs e) instead", false)]
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
        }

        [Obsolete("OnMouseUp(MouseEventArgs e) is not fired by MSTreeView when node is selected.", false)]
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }

        [Obsolete("OnMouseDown(MouseEventArgs e) is not fired by MSTreeView when node is selected.", false)]
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }


        protected virtual void OnBeforeSelect(CMSTreeViewCancelEventArgs e)
        {
            if (!bEnableSelectEvents)
                return;

#if MSTREE_TRACE
			Debug.Write("EVENT: " + traceCount.ToString() + (e.Selected ? " OnBeforeSelect:" : " OnBeforeUnselect:"));
			Debug.Write(" node=" + (e.Node!=null ? "\"" + e.Node.Text + "\"": "null"));
			Debug.WriteLine(" action=" + e.Action);
#endif

            TreeViewCancelEventArgs newE = new TreeViewCancelEventArgs(e.Node, e.Cancel, e.Action);
            if (e.Selected)
            {
                if (onBeforeSelect != null)
                    onBeforeSelect(this, newE);
            }
            else
            {
                if (onBeforeUnselect != null)
                    onBeforeUnselect(this, newE);
            }
            e.Cancel = newE.Cancel;
        }


        protected virtual void OnAfterSelect(CMSTreeViewEventArgs e)
        {
            if (!bEnableSelectEvents)
                return;

#if MSTREE_TRACE
			Debug.Write("EVENT: " + traceCount.ToString() + (e.Selected ? " OnAfterSelect:" : " OnAfterUnselect:"));
			Debug.Write(" node=" + (e.Node!=null ? "\"" + e.Node.Text + "\"" : "null"));
			Debug.WriteLine(" action=" + e.Action);
#endif
            if (e.Selected)
            {
                if (onAfterSelect != null)
                    onAfterSelect(this, new TreeViewEventArgs(e.Node, e.Action));
            }
            else
            {
                if (onAfterUnselect != null)
                    onAfterUnselect(this, new TreeViewEventArgs(e.Node, e.Action));
            }
        }


        protected virtual void OnBeforeMultiSelectStart(CMSTreeViewMultiSelectEventArgs e)
        {
            if (!bEnableSelectEvents)
                return;

#if MSTREE_TRACE
			this.traceCount++;
			Debug.Write("EVENT: " + traceCount.ToString() + " OnBeforeMultiSelectStart:" );
			Debug.WriteLine(" current selection: (" +  ToString(e.SelectedNodes) + ")");
#endif
            if (onBeforeMultiSelectStart != null)
                onBeforeMultiSelectStart(this, e);
        }

        protected virtual void OnAfterMultiSelectComplete(CMSTreeViewMultiSelectEventArgs e)
        {
            if (!bEnableSelectEvents)
                return;

#if MSTREE_TRACE
			Debug.Write("EVENT: " + traceCount.ToString() + " OnAfterMultiSelectComplete:" );
			Debug.WriteLine(" new selection: (" +  ToString(e.SelectedNodes) + ")");
#endif
            if (onAfterMultiSelectComplete != null) onAfterMultiSelectComplete(this, e);
        }

#if (TRACE || DEBUG)
        protected static string ToString(ArrayList nodes)
        {
            if (nodes == null)
                return "Count: null";

            string res = "";
            for (int i = 0; i < nodes.Count; i++)
            {
                if (res != "")
                    res += ", ";

                if (!(nodes[i] is TreeNode))
                {
                    res += "InvalidType(" + nodes[i].GetType().Name + ")";
                    continue;
                }

                res += (nodes[i] != null ? "\"" + ((TreeNode)nodes[i]).Text + "\"" : "null");
            }

            res = "Count: " + nodes.Count + " nodes: " + res;
            return res;
        }
#endif

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            //	components = new System.ComponentModel.Container();
        }
        #endregion

        public ArrayList SelectedNodes
        {
            get
            {
                return (ArrayList)selectedNodes.Clone();
            }
            set
            {
                if (!IsHandleCreated)
                {
                    selectedNodes.Clear();
                    if (value != null)
                        selectedNodes.AddRange(value);
                }
                else
                    ExtendSelection(value, false, true/*select*/, false /*don't select subtree*/, TreeViewAction.Unknown);
            }
        }

        // allow empty selection to drop selection when user clicks outside any node
        public bool AllowEmptySelection
        {
            get
            {
                return this.allowEmptySelection;
            }
            set
            {
                this.allowEmptySelection = value;
            }
        }

        public bool AllowMultiSelection
        {
            get
            {
                return this.allowMultiSelection;
            }
            set
            {
                this.allowMultiSelection = value;
            }
        }


        //[Obsolete("Use FocusedNode instead", false)]
        public new TreeNode SelectedNode
        {
            get
            {
                return FocusedNode;
            }
            set
            {
                FocusedNode = value;
            }
        }

        // returns focused node. 
        // difference between FocusedNode and SelectedNode is focused node can be unselected.
        public TreeNode FocusedNode
        {
            get
            {
                if (IsHandleCreated)
                {
                    TreeNode node = GetFocusedItem();

                    // node can be absent in selectedNodes list, for example, when node is focused but not selected (press CtrlKey to repro it)
                    //Debug.Assert(node == null || (node.TreeView == this && selectedNodes.Contains(node)), "selectedNodes collection doesn't have this node.");
                    return node;
                }
                else if (this.selectedNodes != null && this.selectedNodes.Count > 0)
                {
                    return (TreeNode)selectedNodes[0];
                }
                else
                {
                    return null;
                }
            }

            set
            {
                if (IsHandleCreated && (value == null || value.TreeView == this))
                {
                    SetFocusedItem(value);
                }
                else
                {
                    selectedNodes.Clear();
                    selectedNodes.Add(value);
                }
            }
        }


        private void AddUnselectedNodes(ArrayList nodelist, TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (!node.IsSelected)
                    nodelist.Add(node);

                if (node.Nodes.Count > 0)
                    AddUnselectedNodes(nodelist, node.Nodes);
            }
        }

        public ArrayList UnselectedNodes
        {
            get
            {
                ArrayList nodelist = new ArrayList();
                this.AddUnselectedNodes(nodelist, this.Nodes);
                return nodelist;
            }
        }


        // returns:
        //	<0 - node1<node2
        //	=0 - node1==node2
        //	>0 - node1>node2
        public int CompareNodes(TreeNode node1, TreeNode node2)
        {
            if (node1 == null || node2 == null)
                throw new ArgumentNullException("one of nodes is null");

            ArrayList nodePath1 = RootPath(node1);
            ArrayList nodePath2 = RootPath(node2);

            // let's find first not-related node in the node paths starting from the root;
            // toor node is at the end of the array lists
            int pos1 = nodePath1.Count - 1;
            int pos2 = nodePath2.Count - 1;
            for (; pos1 >= 0 && pos2 >= 0 && nodePath1[pos1] == nodePath2[pos2]; pos1--, pos2--) ;

            if (pos1 < 0)
            {
                // leaves with the same parent
                if (pos2 < 0)
                {
                    Debug.Assert(nodePath1.Count > 0 && nodePath2.Count > 0);
                    return ((TreeNode)nodePath1[0]).Index - ((TreeNode)nodePath2[0]).Index;
                }

                    // node1 is the parent of node2, parent is always less than its children
                else
                    return -1;
            }

            // node2 is the parent of node1, parent is always less than its children
            if (pos2 < 0)
                return +1;

            // node1 and node2 are sisters/brothers, ckeck their indexes
            return ((TreeNode)nodePath1[pos1]).Index - ((TreeNode)nodePath2[pos2]).Index;
        }

        // add a node to the current selection.
        // the function simulates behaviour as a user holds SHIFT or CONTROL keys
        public void ExtendSelection(TreeNode node, bool bShiftKey, bool bControlKey)
        {
            ExtendSelection(node, bShiftKey, bControlKey, TreeViewAction.Unknown);
        }

        // add a node list to the current selection.
        public void ExtendSelection(IEnumerable nodeList, bool bControlKey, bool bSelect, bool bIncludeSubTrees)
        {
            ExtendSelection(nodeList, bControlKey, bSelect, bIncludeSubTrees, TreeViewAction.Unknown);
        }

        // add a node list to the current selection.
        public void ExtendSelection(IEnumerable nodeList, bool bControlKey, bool bSelect)
        {
            ExtendSelection(nodeList, bControlKey, bSelect, false, TreeViewAction.Unknown);
        }

        // implementation part   
        protected void ExtendSelection(TreeNode node, bool bShiftKey, bool bControlKey, TreeViewAction action)
        {
            if (!IsHandleCreated)
                throw new NotImplementedException("set SelectedNodes is not implemented when IsHandleCreated is false");

            OnBeforeMultiSelectStart(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, action));
            DoSelect(node, bShiftKey, bControlKey, action);
            OnAfterMultiSelectComplete(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, action));
        }


        protected void ExtendSelection(IEnumerable nodeList, bool bControlKey, bool bSelect, bool bIncludeSubTrees, TreeViewAction action)
        {
            if (!IsHandleCreated)
                throw new NotImplementedException("set SelectedNodes is not implemented when IsHandleCreated is false");

            OnBeforeMultiSelectStart(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, action));
            if (!bControlKey)
                DoSelect(null as TreeNode, false, false, action);

            DoSelect(nodeList, bSelect, bIncludeSubTrees, action);
            OnAfterMultiSelectComplete(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, action));
        }


        private void ClearSelection()
        {
            for (int i = this.selectedNodes.Count - 1; i >= 0; i--)
            {
                TreeNode node = (TreeNode)this.selectedNodes[i];
                //SelectedNodes.RemoveAt(i);
                SetItemState(node, 0, Native.TVIS_SELECTED);
            }
        }

        private ArrayList RootPath(TreeNode node)
        {
            ArrayList arr = new ArrayList();
            for (; node != null; node = node.Parent)
                arr.Add(node);
            return arr;
        }


        private void DoSelect(IEnumerable nodeList, bool bSelect, bool bIncludeSubTrees, TreeViewAction action)
        {
            foreach (TreeNode node in nodeList)
            {
                if (node.TreeView != this)
                    throw new ArgumentException("Can't select a node from another tree", node.Text);
                if (IsSelectedState(node) != bSelect)
                    DoSelect(node, false, true, action);

                if (bIncludeSubTrees && node.Nodes.Count > 0)
                    DoSelect(node.Nodes, bSelect, bIncludeSubTrees, action);
            }
        }

        private void DoSelect(TreeNode node, bool bShift, bool bControl, TreeViewAction action)
        {
            if (node == null)
            {
                // drop selection
                ClearSelection();
                //SelectedNode = null;
                return;
            }

            // user clicks an item and holds <Control> or/and <Shift>
            // --------input--------------	----output -----
            // Control	Shift	IsSelected	will_be_selected	
            //	+		+		+				-
            //	+		+		-				+
            //	+		-		+				-
            //	+		-		-				+
            //	-		+		+				+
            //	-		+		-				+
            //	-		-		+				+
            //	-		-		-				+

            bool bIsSelected = IsSelectedState(node);
            bool bWillBeSelected = !bControl || !bIsSelected;

            // save the current focused item
            TreeNode focusedNode = GetFocusedItem();

            if (focusedNode != null && focusedNode.Handle != node.Handle)
            {
                // save state of the focused node
                bool bWasFocusedNodeSelected = IsSelectedState(focusedNode);

                // select item
                if (SetFocusedItem(node, bWillBeSelected, focusedNode, action))
                {
                    // restore state of the previous focused item
                    if (bWasFocusedNodeSelected)
                        SetSelectedState(focusedNode, true);
                }
                else
                    return;
            }

            else
            {
                if (!SetFocusedItem(node, bWillBeSelected, focusedNode, action))
                    return;
            }

            // single operation
            if (focusedNode == null || !this.allowMultiSelection)
                return;

            // group operation
            bool bFocusedItemIsLess = CompareNodes(focusedNode, node) < 0;
            if (bShift)
            {
                // expand selection
                if (focusedNode.Handle != node.Handle)
                {
                    if (bFocusedItemIsLess)
                    {
                        for (TreeNode j = focusedNode.NextVisibleNode; j != null && j.Handle != node.Handle; j = j.NextVisibleNode)
                            SetSelectedState(j, true);
                    }
                    else
                    {
                        for (TreeNode j = node.NextVisibleNode; j != null && j.Handle != focusedNode.Handle; j = j.NextVisibleNode)
                            SetSelectedState(j, true);
                    }
                }
            }

            if (!bControl)
            {
                TreeNode hMin = node, hMax = node;

                // if nFlags contains MK_SHIFT (without MK_CONTROL)
                // unselect items disconnected from the [hFocusedItem, hItem] interval 
                if (bShift)
                {
                    if (bFocusedItemIsLess)
                    {
                        hMin = focusedNode; hMax = node;
                    }
                    else
                    {
                        hMax = focusedNode; hMin = node;
                    }

                    // find the beginning of selection group containing [hFocusedItem, hItem] interval
                    for (TreeNode j = hMin.PrevVisibleNode; j != null && IsSelectedState(j); j = j.PrevVisibleNode)
                        hMin = j;

                    // find the end of selection group containing [hFocusedItem, hItem] interval
                    for (TreeNode j = hMax.NextVisibleNode; j != null && IsSelectedState(j); j = j.NextVisibleNode)
                        hMax = j;
                }

                if (this.selectedNodes.Count > 0)
                { // user could cancel selection of some nodes, so we need to check size of selectedNodes array
                    // unselect all nodes that are greater than hMax and less hMin
                    while (this.selectedNodes.Count > 0 && this.selectedNodes[0] != hMin)
                    {
                        // changing node state updates the selectedNode array
                        SetSelectedState((TreeNode)this.selectedNodes[0], false);
                    }
                    //Debug.Assert(selectedNodes[0] == hMin, "the selectedNodes array doesn't contain selected element.");

                    for (int i = this.selectedNodes.Count - 1; i >= 0 && this.selectedNodes[i] != hMax; i--)
                    {
                        // changing node state updates the selectedNode array
                        SetSelectedState((TreeNode)this.selectedNodes[i], false);
                    }
                    //Debug.Assert(selectedNodes[i] == hMax, "the selectedNodes array doesn't contain selected element.");
                }
            }
        }

        private void AddSelectedItem(TreeNode node)
        {
            if (node == null)
                return;

            if (!this.selectedNodes.Contains(node))
            {
                int i = 0;
                for (; i < this.selectedNodes.Count && CompareNodes((TreeNode)this.selectedNodes[i], node) < 0; i++) { }
                this.selectedNodes.Insert(i, node);
            }
        }

        private void RemoveSelectedItem(TreeNode node)
        {
            if (this.selectedNodes.Contains(node))
                this.selectedNodes.Remove(node);
        }

        private void RemoveDestroyedSelectedItems()
        {
            // remove nodes with Handle==null
            for (int i = 0; i < this.selectedNodes.Count; i++)
            {
                if (((TreeNode)this.selectedNodes[i]).Handle == IntPtr.Zero)
                {
                    int j = i + 1;
                    for (; j < this.selectedNodes.Count &&
                        ((TreeNode)this.selectedNodes[j]).Handle == IntPtr.Zero; j++) { }

                    this.selectedNodes.RemoveRange(i, j - i);
                    i = j;
                }
            }
        }

        private IntPtr SetItemState(TreeNode node, int state, int stateMask)
        {
            Native.TV_ITEM lti = new Native.TV_ITEM();
            lti.hItem = node != null ? node.Handle : IntPtr.Zero;
            lti.mask = Native.TVIF_STATE | Native.TVIF_HANDLE;
            lti.stateMask = stateMask;
            lti.state = state;
            return Native.SendMessage(this.Handle, Native.TVM_SETITEM, 0, ref lti);
        }

        private int GetItemState(TreeNode node, int stateMask)
        {
            Native.TV_ITEM lti = new Native.TV_ITEM();
            lti.hItem = node.Handle;
            lti.mask = Native.TVIF_STATE | Native.TVIF_HANDLE;
            lti.stateMask = stateMask;
            lti.state = 0;

            Native.SendMessage(this.Handle, Native.TVM_GETITEM, 0, ref lti);
            return lti.state;
        }

        private IntPtr SetSelectedState(TreeNode node, bool bSelected)
        {
            return SetItemState(node, bSelected ? Native.TVIS_SELECTED : 0, Native.TVIS_SELECTED);
        }

        private bool IsSelectedState(TreeNode node)
        {
            return (GetItemState(node, Native.TVIS_SELECTED) & Native.TVIS_SELECTED) != 0;
        }

        private IntPtr SetFocusedItem(TreeNode node)
        {
            IntPtr hItem = (node == null) ? IntPtr.Zero : node.Handle;
            return Native.SendMessage(this.Handle, Native.TVM_SELECTITEM, Native.TVGN_CARET, hItem);
        }

        private bool SetFocusedItem(TreeNode node, bool bSelect, TreeNode oldNode, TreeViewAction action)
        {
            CMSTreeViewCancelEventArgs e = new CMSTreeViewCancelEventArgs(node, false, bSelect, action);
            OnBeforeSelect(e);
            if (e.Cancel)
                return false;

            // temporary disable OnXXXSelect events
            this.bEnableSelectEvents = false;
            IntPtr hItem = SetFocusedItem(node);

            if (bSelect)
                AddSelectedItem(node);
            else
                RemoveSelectedItem(node);
            SetSelectedState(node, bSelect);

            // enable OnXXXSelect events
            this.bEnableSelectEvents = true;

            OnAfterSelect(new CMSTreeViewEventArgs(node, bSelect, action));
            return true;
        }

        private TreeNode GetFocusedItem()
        {
            IntPtr hItem = Native.SendMessage(this.Handle, Native.TVM_GETNEXTITEM, Native.TVGN_CARET, (IntPtr)0);
            if (hItem == IntPtr.Zero)
                return null;

            TreeNode node = TreeNode.FromHandle(this, hItem);
            return node;
        }

        private unsafe bool TvnSelectItem(ref Message m)
        {
            Native.TV_ITEM* item = (Native.TV_ITEM*)m.LParam;

            // Check for invalid node handle
            if (item == null || item->hItem == IntPtr.Zero)
                return false;

            if ((item->mask & Native.TVIF_STATE) == 0 || (item->stateMask & Native.TVIS_SELECTED) == 0)
                return false;

            TreeNode node = TreeNode.FromHandle(this, item->hItem);
            Debug.Assert(node != null);

            bool bIsSelected = (item->state & Native.TVIS_SELECTED) != 0;
            bool bDontFireEvents = bIsSelected == this.selectedNodes.Contains(node);

            if (!bDontFireEvents)
            {
                CMSTreeViewCancelEventArgs e = new CMSTreeViewCancelEventArgs(node, false, bIsSelected, TreeViewAction.Unknown);
                OnBeforeSelect(e);

                if (e.Cancel)
                {
                    m.Result = (IntPtr)1;
                    return true;
                }

                if (bIsSelected) AddSelectedItem(node);
                else
                    RemoveSelectedItem(node);
            }

            base.WndProc(ref m);

            if (!bDontFireEvents)
            {
                OnAfterSelect(new CMSTreeViewEventArgs(node, bIsSelected, TreeViewAction.Unknown));
            }

            return true;
        }

        private unsafe bool WmSelChanging(ref Message m)
        {
            if (!bEnableSelectEvents)
                return false;

            Native.NMTREEVIEW* nmtv = (Native.NMTREEVIEW*)m.LParam;

            TreeNode node = TreeNode.FromHandle(this, nmtv->itemNew.hItem);

            OnBeforeMultiSelectStart(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, TreeViewAction.Unknown));
            bool bIsSelected = true;

            CMSTreeViewCancelEventArgs e = new CMSTreeViewCancelEventArgs(node, false, bIsSelected, TreeViewAction.Unknown);
            OnBeforeSelect(e);
            if (e.Cancel)
            {
                // user canceled operation, cancel changing of the selection 
                m.Result = (IntPtr)1;
                return true;
            }

            return false;
        }

        private unsafe bool WmSelChanged(ref Message m)
        {
            if (!bEnableSelectEvents)
                return false;

            Native.NMTREEVIEW* nmtv = (Native.NMTREEVIEW*)m.LParam;

            TreeNode node = TreeNode.FromHandle(this, nmtv->itemNew.hItem);
            bool bIsSelected = true;//(nmtv->itemNew.state & Native.TVIS.SELECTED) != 0;
            TreeNode oldNode = TreeNode.FromHandle(this, nmtv->itemOld.hItem);

            AddSelectedItem(node);
            CMSTreeViewEventArgs e = new CMSTreeViewEventArgs(node, bIsSelected, TreeViewAction.Unknown);
            OnAfterSelect(e);

            OnAfterMultiSelectComplete(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, TreeViewAction.Unknown));

            return false;
        }

        private unsafe bool WmExpanding(ref Message m)
        {
            Native.NMTREEVIEW* nmtv = (Native.NMTREEVIEW*)m.LParam;

            bool bCollapse = nmtv->action == Native.TVE_COLLAPSE;
            if (bCollapse)
            {
                TreeNode node = TreeNode.FromHandle(this, nmtv->itemNew.hItem);
                if (node.GetNodeCount(false) == 0)
                    return false;

                TreeNode firstChild = node.FirstNode;
                TreeNode nextSibling = node.NextNode;
                TreeNode focusedNode = GetFocusedItem();

                bool bCancel = false; // cancel event processing

                // change focus if necessary
                bool bChangeSelection = CompareNodes(focusedNode, firstChild) >= 0 &&
                    (nextSibling == null || CompareNodes(focusedNode, nextSibling) < 0);
                if (bChangeSelection)
                {
                    OnBeforeMultiSelectStart(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, TreeViewAction.Collapse));

                    if (!SetFocusedItem(node, true, focusedNode, TreeViewAction.Collapse))
                    {
                        // user canceled operation, cancel collapsing
                        bCancel = true;
                        goto cleanup;
                    }
                }

                // get the first selected child node
                int i = 0;
                for (; i < this.selectedNodes.Count; i++)
                {
                    if (CompareNodes((TreeNode)this.selectedNodes[i], firstChild) >= 0)
                        break;
                }
                if (i >= this.selectedNodes.Count)
                    goto cleanup;

                // drop all selected nodes starting from i
                for (; i < this.selectedNodes.Count && (nextSibling == null || CompareNodes((TreeNode)this.selectedNodes[i], nextSibling) < 0); )
                {
                    if (!bChangeSelection)
                    {
                        // OnBeforeMultiSelectStart should be called only once
                        OnBeforeMultiSelectStart(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, TreeViewAction.Collapse));
                        bChangeSelection = true;
                    }

                    // drop selection
                    int count = this.selectedNodes.Count;
                    SetSelectedState((TreeNode)this.selectedNodes[i], false);
                    int countAfter = this.selectedNodes.Count;

                    if (countAfter == count)
                    {
                        // user canceled operation, cancel collapsing
                        m.Result = (IntPtr)1;
                        break;
                    }

                    // SetSelectedState changes selectedNodes array, so we don't need to shift i
                }

            cleanup: ;
                if (bChangeSelection)
                    OnAfterMultiSelectComplete(new CMSTreeViewMultiSelectEventArgs(SelectedNodes, TreeViewAction.Collapse));

                if (bCancel)
                    bCancel = true;

                return bCancel;
            }
            return false;
        }

        private unsafe bool WmNotify(ref Message m)
        {
            Native.NMHDR* nmhdr = (Native.NMHDR*)m.LParam;

            // Custom draw code is handled separately.
            //
            if (nmhdr->code == (int)Native.NotifyMsg.NM_CUSTOMDRAW)
                return false;

            Native.NMTREEVIEW* nmtv = (Native.NMTREEVIEW*)m.LParam;
            switch (nmtv->nmhdr.code)
            {
                //case Native.TVN_DELETEITEMA:
                //case Native.TVN_DELETEITEMW:
                //	return WmDeleteItem(ref m);

                case Native.TVM_SETITEMA:
                case Native.TVM_SETITEMW:
                    break;

                case Native.TVN_ITEMEXPANDEDA:
                case Native.TVN_ITEMEXPANDEDW:
                    break;

                case Native.TVN_ITEMEXPANDINGA:
                case Native.TVN_ITEMEXPANDINGW:
                    return WmExpanding(ref m);

                case Native.TVN_SELCHANGINGA:
                case Native.TVN_SELCHANGINGW:
                    return WmSelChanging(ref m);

                case Native.TVN_SELCHANGEDA:
                case Native.TVN_SELCHANGEDW:
                    return WmSelChanged(ref m);
            }

            return false;
        }

        private unsafe bool WmDeleteItem(ref Message m)
        {
            //Native.NMTREEVIEW* nmtv = (Native.NMTREEVIEW*)m.LParam;
            //IntPtr hItem = nmtv->itemOld.hItem; 
            IntPtr hItem = m.LParam;
            TreeNode node = TreeNode.FromHandle(this, hItem);
            if (node != null)
            {
                RemoveDestroyedSelectedItems();
                RemoveSelectedItem(node);
            }
            return false;
        }

        private bool WmKeyDown(ref Message m)
        {
            int nChar = m.WParam.ToInt32();
            switch (nChar)
            {
                case (int)Native.VK.PAGEUP:
                case (int)Native.VK.PAGEDOWN:
                case (int)Native.VK.HOME:
                case (int)Native.VK.END:
                    ExtendSelection(null as TreeNode, false, false, TreeViewAction.ByKeyboard);
                    return false;

                case (int)Native.VK.UP:
                case (int)Native.VK.DOWN:
                    {
                        TreeNode focusedNode = GetFocusedItem();
                        if (focusedNode == null)
                            return false;

                        TreeNode nextNode = (nChar == (int)Native.VK.UP) ? focusedNode.PrevVisibleNode : focusedNode.NextVisibleNode;
                        if (nextNode == null)
                            return false;

                        bool bShift = (Native.GetKeyState((int)Native.VK.SHIFT) & 0x8000) != 0;
                        ExtendSelection(nextNode, bShift, false, TreeViewAction.ByKeyboard);
                        return true;
                    }
            }
            return false;
        }

        private bool WmRButtonDown(ref Message m)
        {
            if (!ContainsFocus)
                Focus();

            Native.TV_HITTESTINFO tvhip = new Native.TV_HITTESTINFO();
            tvhip.pt_x = (int)(short)m.LParam;
            tvhip.pt_y = ((int)m.LParam >> 16);
            IntPtr handlenode = Native.SendMessage(this.Handle, Native.TVM_HITTEST, 0, tvhip);

            OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, (int)(short)m.LParam, (int)m.LParam >> 16, 0));
            if ((tvhip.flags & Native.TVHT_ONITEM) != 0)
            {
                TreeNode node = TreeNode.FromHandle(this, tvhip.hItem);
                if (this.allowEmptySelection && !IsSelectedState(node))
                    ExtendSelection(node, false, false, TreeViewAction.ByMouse);

            }

            // show context menu
            Native.SendMessage(Handle, (int)Native.Msg.WM_CONTEXTMENU, (int)Handle, (IntPtr)Native.GetMessagePos());
            m.Result = (IntPtr)1;
            return true;
        }

        private bool WmLButtonDown(ref Message m)
        {
            if (!ContainsFocus)
                Focus();

            Native.TV_HITTESTINFO tvhip = new Native.TV_HITTESTINFO();
            tvhip.pt_x = (int)(short)m.LParam;
            tvhip.pt_y = ((int)m.LParam >> 16);
            IntPtr handlenode = Native.SendMessage(this.Handle, Native.TVM_HITTEST, 0, tvhip);

            int keys = (int)m.WParam;
            if ((tvhip.flags & Native.TVHT_ONITEM) != 0)
            {
                TreeNode node = null;
                if (handlenode == IntPtr.Zero)
                {
                    Debug.Assert(false);
                    return false;
                }
                node = TreeNode.FromHandle(this, handlenode);

                OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, (int)(short)m.LParam, (int)m.LParam >> 16, 0));
                ExtendSelection(node, (keys & Native.MK_SHIFT) != 0, (keys & Native.MK_CONTROL) != 0, TreeViewAction.ByMouse);
            }
            else if ((tvhip.flags & Native.TVHT_ONITEMBUTTON) != 0)
            { // collapse/expand
                // use default handler
                return false;
            }
            else
            {
                if (this.allowEmptySelection)
                    ExtendSelection(null as TreeNode, (keys & Native.MK_SHIFT) != 0, (keys & Native.MK_CONTROL) != 0, TreeViewAction.ByMouse);
                return false;
            }
            return true;
        }


        protected override void WndProc(ref Message m)
        {
            bool bHandled = false;
            switch (m.Msg)
            {
                case (int)Native.Msg.WM_REFLECT + (int)Native.Msg.WM_NOTIFY:
                    bHandled = WmNotify(ref m);
                    break;

                case (int)Native.Msg.WM_KEYDOWN:
                    {
                        bHandled = WmKeyDown(ref m);
                        break;
                    }

                case Native.TVM_SETITEMA:
                case Native.TVM_SETITEMW:
                    {
                        bHandled = TvnSelectItem(ref m);
                        break;
                    }

                case (int)Native.Msg.WM_KILLFOCUS:
                case (int)Native.Msg.WM_SETFOCUS:
                    {
                        Invalidate();
                        break;
                    }

                case (int)Native.Msg.WM_LBUTTONDOWN:
                    {
                        bHandled = WmLButtonDown(ref m);
                        break;
                    }

                case (int)Native.Msg.WM_RBUTTONDOWN:
                    {
                        bHandled = WmRButtonDown(ref m);
                        break;
                    }
            }

            if (!bHandled)
                base.WndProc(ref m);

            switch (m.Msg)
            {
                case (int)Native.Msg.WM_CREATE:
                    {
                        ArrayList tmp = (ArrayList)this.selectedNodes.Clone();
                        this.selectedNodes.Clear();
                        SelectedNodes = tmp;
                        break;
                    }

                case Native.TVM_DELETEITEM:
                    {
                        WmDeleteItem(ref m);
                        break;
                    }
            }
        }
    }

    /// <summary>
    /// CMSTreeViewEventArgs
    /// </summary>
    public class CMSTreeViewEventArgs : TreeViewEventArgs
    {
        protected bool bSelected;
        public CMSTreeViewEventArgs(TreeNode node, bool bSelected, TreeViewAction action)
            : base(node, action)
        {
            this.bSelected = bSelected;
        }

        public bool Selected
        {
            get
            {
                return bSelected;
            }
        }
    }

    /// <summary>
    /// CMSTreeViewCancelEventArgs 
    /// </summary>
    public class CMSTreeViewCancelEventArgs : TreeViewCancelEventArgs
    {
        protected bool bSelected;
        public CMSTreeViewCancelEventArgs(TreeNode node, bool bCancel, bool bSelected, TreeViewAction action)
            : base(node, bCancel, action)
        {
            this.bSelected = bSelected;
        }

        public bool Selected
        {
            get
            {
                return bSelected;
            }
        }
    }

    /// <summary>
    /// CMSTreeViewMultiSelectEventArgs
    /// </summary>
    public class CMSTreeViewMultiSelectEventArgs : EventArgs
    {
        protected ArrayList nodeList;
        protected TreeViewAction action;
        public CMSTreeViewMultiSelectEventArgs(ArrayList nodeList, TreeViewAction action)
        {
            this.nodeList = nodeList;
            this.action = action;
        }

        public ArrayList SelectedNodes
        {
            get
            {
                return nodeList;
            }
        }

        public TreeViewAction Action
        {
            get
            {
                return action;
            }
        }
    }

    /// <summary>
    /// Event handlers
    /// </summary>
    public delegate void CMSTreeViewEventHandler(object sender, CMSTreeViewEventArgs e);
    public delegate void CMSTreeViewCancelEventHandler(object sender, CMSTreeViewCancelEventArgs e);
    public delegate void CMSTreeViewMultiSelectCompleteEventHandler(object sender, CMSTreeViewMultiSelectEventArgs e);
    public delegate void CMSTreeViewMultiSelectStartEventHandler(object sender, CMSTreeViewMultiSelectEventArgs e);
}

#endif
