using System;
using System.Collections.Generic;
using Tang;
using UnityEditor;
using UnityEngine;

namespace ZS
{
    public class CreatorTreeNode
    {
        
        public int treeIndex = 0;
        private bool IsRepair = false;
        public TreeNode currentNode;

        public Action CreateTreeNode;

        public void SetCurrentNode(TreeNode root)
        {
            treeIndex = 0;
            currentNode = root;
        }
        
        
        public void DrawFileTree(TreeNode node, Rect rect,int level =0)
        {
            if (node == null) 
            {
                return;
            }
            GUIStyle style = new GUIStyle("Label");
            style.normal.background = null;
            if (node == currentNode) 
            {
                style.normal.textColor = Color.cyan;
            }
 
            Rect BaseRect = new Rect(5+5*level+rect.x, 5+20*treeIndex+rect.y, rect.width- 15*2, 20);
            Rect FoldoutRect = new Rect(BaseRect.x, BaseRect.y, 15, BaseRect.height);
            Rect BtnRect = new Rect(BaseRect.x+15, BaseRect.y, BaseRect.width -15, BaseRect.height);

            treeIndex++;

            
            using (new EditorGUILayout.VerticalScope())
            {
                if (node.nodeType == TreeNode.TreeNodeType.Switch) {
                    node.isOpen = EditorGUI.Foldout (FoldoutRect, node.isOpen, "", true);
                    if (GUI.Button (BtnRect, node.nodeSkillData.id, style)) 
                    {
                        currentNode = node;
                    }
                }
                else
                {
                    if (GUI.Button (BtnRect, node.nodeSkillData.id, style)) 
                    {
                        currentNode = node;
                    }
                }

                RightDrop(BtnRect);
            }
            
	
            if (node==null || !node.isOpen || node.children == null) 
            {
                return;
            }
		
            foreach (var childTree in node.children)
            {
                DrawFileTree (childTree,rect,level+1);
            }

            if (!IsRepair) return;
            IsRepair = false;
            Creat();

        }
        
        private void RightDrop(Rect btnRect)
        {
            MyGUIExtend.Instance.Single_RightDrop(btnRect,new Dictionary<string, Action>
            {
                {
                    "添加", () =>
                    {
                        var newSkill = new SkillData {id = "New"};
                        currentNode.nodeSkillData.skillDatas.Add(newSkill);
                        IsRepair = true;
                    }
                },
                {
                    "删除", () =>
                    {
                        currentNode.parent.nodeSkillData.skillDatas.Remove(currentNode.nodeSkillData);
                        IsRepair = true;
                    }
                }
            });
        }

        private void Creat()
        {
            CreateTreeNode?.Invoke();
        }
    }
}