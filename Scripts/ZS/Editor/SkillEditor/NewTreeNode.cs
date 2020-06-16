using System.Collections.Generic;

namespace ZS
{
    public class NewTreeNode
    {
        public enum NewTreeNodeType
        {
            Item,
            Switch
        }
        
        public string name;
        public NewTreeNodeType nodeType = NewTreeNodeType.Item;
        public NewTreeNode parent;
        public List<NewTreeNode> children = null;
        public bool isOpen = false;
        public object nodeObject;
        
        
        public void InsertNode(NewTreeNode node)
        {
            if (this.children == null)
            {
                this.children = new List<NewTreeNode> ();
            }
            children.Add (node);
            node.parent = this;
        }
        
        public void OpenAllNode(NewTreeNode node)
        {
            node.isOpen = true;
            if (node.children != null && node.children.Count > 0) 
            {
                for (int i = 0; i < node.children.Count; i++) 
                {
                    OpenAllNode (node.children[i]);
                }
            }
        }
        
//        public TreeNode CreatSkillTree(object obj)
//        {
//            NewTreeNode node = new NewTreeNode {nodeObject = obj};
//            if (obj.skillDatas.Count>0)
//            {
//                node.nodeType = TreeNodeType.Switch;
//                for (int i = 0; i < obj.skillDatas.Count; i++)
//                {
//                    var item = obj.skillDatas[i];
//                    TreeNode childNode = CreatSkillTree(item);
//                    node.InsertNode(childNode);
//                }
//            }
//            else
//            {
//                node.nodeType = TreeNodeType.Item;
//            }
//
//            return node;
//        }
        
        
    }
}