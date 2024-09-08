using System;
using FairyGUI;

namespace ET.Client
{
    public static class FUIGListHelper
    {
        /// <summary>
        /// 初始化GList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onRefreshItem"></param>
        /// <param name="onClickItem"></param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        public static void Init(this GList list, Action<int, GObject> onRefreshItem, Action<int, GObject> onClickItem = null, int itemNums = -1,
        int selectedIndex = -1)
        {
            list.itemRenderer = ((index, obj) => { onRefreshItem.Invoke(index, obj); });

            if (onClickItem != null)
            {
                list.onClickItem.Set((content) =>
                {
                    GObject item = content.data as GObject;
                    int childIndex = list.GetChildIndex(item);
                    onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), item);
                });

                if (itemNums > -1)
                {
                    list.numItems = itemNums;
                }

                if (list.numItems != 0 && selectedIndex > -1)
                {
                    int childIndex = list.ItemIndexToChildIndex(selectedIndex);
                    GObject item = list.GetChildAt(childIndex);
                    item.asButton.selected = true;
                    onClickItem.Invoke(selectedIndex, item);
                }
            }
        }
        
        /// <summary>
        /// 异步初始化GList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onRefreshItem"></param>
        /// <param name="onClickItem"></param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        public static async ETTask InitAsync(this GList list, Func<int, GObject, ETTask> onRefreshItem, Func<int, GObject, ETTask> onClickItem = null, int itemNums = -1,
        int selectedIndex = -1)
        {
            list.itemRenderer = (async (index, obj) => { await onRefreshItem.Invoke(index, obj); });

            if (onClickItem != null)
            {
                list.onClickItem.Set(async (content) =>
                {
                    GObject item = content.data as GObject;
                    int childIndex = list.GetChildIndex(item);
                    await onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), item);
                });

                if (itemNums > -1)
                {
                    list.numItems = itemNums;
                }

                if (list.numItems != 0 && selectedIndex > -1)
                {
                    int childIndex = list.ItemIndexToChildIndex(selectedIndex);
                    GObject item = list.GetChildAt(childIndex);
                    item.asButton.selected = true;
                    await onClickItem.Invoke(selectedIndex, item);
                }
            }
        }

        /// <summary>
        /// 初始化TreeList（仅限2层结构）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onRefreshItem">刷新文件夹和页签回调</param>
        /// <param name="onClickItem">点击页签回调</param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="multiSelected">tree：文件夹可多选 false：文件夹单选</param>
        public static void InitTree(this GTree list, Action<int, GTreeNode> onRefreshItem, Action<int, GObject> onClickItem, int itemNums = -1,
        int selectedIndex = -1, bool multiSelected = false)
        {
            list.treeNodeRender = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(index, treeNode);
            };

            list.treeNodeWillExpand = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(index, treeNode);

                int startIndex = index + 1;
                if (index > 0)
                {
                    for (int i = 0; i < index; i++)
                    {
                        startIndex += list.rootNode.GetChildAt(i).numChildren;
                    }
                }

                for (int i = 0; i < treeNode.numChildren; i++)
                {
                    GTreeNode item = treeNode.GetChildAt(i);
                    onRefreshItem.Invoke(i + startIndex, item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        onClickItem.Invoke(i + startIndex, item.cell);
                    }
                }
            };

            EventCallback1 listener = (context) =>
            {
                GObject item = (GObject)context.data;

                int childIndex = list.GetChildIndex(item);
                int index = list.ChildIndexToItemIndex(childIndex);

                GTreeNode node = item.treeNode;
                if (node.isFolder)
                {
                    //单选时, 点击已经展开的文件夹,不会收起它
                    if (!multiSelected)
                    {
                        for (int i = list.rootNode.numChildren - 1; i >= 0; i--)
                        {
                            GTreeNode tarNode = list.rootNode.GetChildAt(i);
                            tarNode.expanded = tarNode == node;
                        }
                    }

                    //选中文件夹下第一个页签按钮
                    node.GetChildAt(0).cell.asButton.selected = true;
                }
                else
                {
                    int pIndex = list.rootNode.GetChildIndex(node.parent);

                    for (int i = 0; i < pIndex; i++)
                    {
                        index += list.rootNode.GetChildAt(i).numChildren;
                    }

                    item.asButton.selected = true;
                    //选中文件夹下第一个页签
                    onClickItem.Invoke(index, item);
                }
            };

            list.onClickItem.Set(listener);

            if (itemNums > -1)
            {
                list.numItems = itemNums;
            }
            else
            {
                itemNums = list.numItems;
            }

            if (itemNums != 0 && selectedIndex > -1)
            {
                GTreeNode treeNode = list.rootNode.GetChildAt(selectedIndex);

                //展开文件夹
                list.treeNodeWillExpand(treeNode, true);
                //触发点击文件夹事件
                listener.Invoke(new EventContext() { data = treeNode.cell });
            }

            onRefreshItem.Invoke(0, list.rootNode.GetChildAt(0));
        }
    }
}