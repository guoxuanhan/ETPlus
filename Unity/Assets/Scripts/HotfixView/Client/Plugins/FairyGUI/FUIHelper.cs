using System;
using System.Collections.Generic;
using FairyGUI;

namespace ET.Client
{
    public static class FUIHelper
    {
        #region AddListnerAsync(this GObject self, Func<ETTask> action)

        public static void AddListnerAsync(this GObject self, Func<ETTask> action)
        {
            async ETTask ClickActionAsync()
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action();
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set(() =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync().Coroutine();
            });
        }
        
        public static void AddListnerAsync<P1>(this GObject self, Func<P1, ETTask> action, P1 p1)
        {
            async ETTask ClickActionAsync()
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(p1);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set(() =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync().Coroutine();
            });
        }
        
        public static void AddListnerAsync<P1, P2>(this GObject self, Func<P1, P2, ETTask> action, P1 p1, P2 p2)
        {
            async ETTask ClickActionAsync()
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(p1, p2);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set(() =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync().Coroutine();
            });
        }
        
        public static void AddListnerAsync<P1, P2 ,P3>(this GObject self, Func<P1, P2, P3, ETTask> action, P1 p1, P2 p2, P3 p3)
        {
            async ETTask ClickActionAsync()
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(p1, p2, p3);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set(() =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync().Coroutine();
            });
        }

        #endregion

        #region AddListner(this GObject self, Action action)

        public static void AddListner(this GObject self, Action action)
        {
            self.onClick.Set(() =>
            {
                action?.Invoke();
            });
        }

        public static void AddListner<P1>(this GObject self, Action<P1> action, P1 p1)
        {
            self.onClick.Set(() =>
            {
                action?.Invoke(p1);
            });
        }
        
        public static void AddListner<P1, P2>(this GObject self, Action<P1, P2> action, P1 p1, P2 p2)
        {
            self.onClick.Set(() =>
            {
                action?.Invoke(p1, p2);
            });
        }
        
        public static void AddListner<P1, P2, P3>(this GObject self, Action<P1, P2, P3> action, P1 p1, P2 p2, P3 p3)
        {
            self.onClick.Set(() =>
            {
                action?.Invoke(p1, p2, p3);
            });
        }

        #endregion

        #region AddListnerAsync(this GObject self, Func<EventContext, ETTask> action)

        public static void AddListnerAsync(this GObject self, Func<EventContext, ETTask> action)
        {
            async ETTask ClickActionAsync(EventContext context)
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(context);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set((context) =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync(context).Coroutine();
            });
        }

        public static void AddListnerAsync<P1>(this GObject self, Func<EventContext, P1, ETTask> action, P1 p1)
        {
            async ETTask ClickActionAsync(EventContext context)
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(context, p1);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set((context) =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync(context).Coroutine();
            });
        }
        
        public static void AddListnerAsync<P1, P2>(this GObject self, Func<EventContext, P1, P2, ETTask> action, P1 p1, P2 p2)
        {
            async ETTask ClickActionAsync(EventContext context)
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(context, p1, p2);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set((context) =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync(context).Coroutine();
            });
        }
        
        public static void AddListnerAsync<P1, P2, P3>(this GObject self, Func<EventContext, P1, P2, P3, ETTask> action, P1 p1, P2 p2, P3 p3)
        {
            async ETTask ClickActionAsync(EventContext context)
            {
                FUIEntitySystemSingleton.Instance.isClicked = true;
                await action(context, p1, p2, p3);
                FUIEntitySystemSingleton.Instance.isClicked = false;
            }

            self.onClick.Set((context) =>
            {
                if (FUIEntitySystemSingleton.Instance.isClicked)
                {
                    return;
                }

                ClickActionAsync(context).Coroutine();
            });
        }

        #endregion

        #region AddListner(this GObject self, Action<EventContext> action)

        public static void AddListner(this GObject self, Action<EventContext> action)
        {
            self.onClick.Set((contex) =>
            {
                action?.Invoke(contex);
            });
        }

        public static void AddListner<P1>(this GObject self, Action<EventContext, P1> action, P1 p1)
        {
            self.onClick.Set((contex) =>
            {
                action?.Invoke(contex, p1);
            });
        }
        
        public static void AddListner<P1, P2>(this GObject self, Action<EventContext, P1, P2> action, P1 p1, P2 p2)
        {
            self.onClick.Set((contex) =>
            {
                action?.Invoke(contex, p1, p2);
            });
        }
        
        public static void AddListner<P1, P2, P3>(this GObject self, Action<EventContext, P1, P2, P3> action, P1 p1, P2 p2, P3 p3)
        {
            self.onClick.Set((contex) =>
            {
                action?.Invoke(contex, p1, p2, p3);
            });
        }

        #endregion

        #region GList AddListner
        
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
                    list.selectedIndex = selectedIndex;
                    onClickItem.Invoke(selectedIndex, item);
                }
            }
        }
        
        /// <summary>
        /// 初始化GList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dataList"></param>
        /// <param name="onRefreshItem"></param>
        /// <param name="onClickItem"></param>
        /// <param name="selectedIndex"></param>
        /// <typeparam name="T"></typeparam>
        public static void Init<T>(this GList list, List<T> dataList, Action<int, T, GObject> onRefreshItem, Action<int, T, GObject> onClickItem = null,
        int selectedIndex = -1)
        {
            if (dataList == null || dataList.Count == 0)
            {
                list.numItems = 0;
                return;
            }

            list.itemRenderer = (index, ojb) => { onRefreshItem.Invoke(index, dataList[index], ojb); };

            if (onClickItem != null)
            {
                list.onClickItem.Set((context) =>
                {
                    GObject item = (GObject)context.data;
                    int childIndex = list.GetChildIndex(item);
                    onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), dataList[list.ChildIndexToItemIndex(childIndex)], item);
                });

                list.numItems = dataList.Count;

                if (list.numItems != 0 && selectedIndex > -1)
                {
                    int childIndex = list.ItemIndexToChildIndex(selectedIndex);
                    GObject item = list.GetChildAt(childIndex);
                    item.asButton.selected = true;
                    list.selectedIndex = selectedIndex;
                    onClickItem.Invoke(selectedIndex, dataList[selectedIndex], item);
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
                    list.selectedIndex = selectedIndex;
                    await onClickItem.Invoke(selectedIndex, item);
                }
            }
        }
        
        /// <summary>
        /// 异步初始化GList
        /// </summary>
        /// <param name="list"></param>
        /// <param name="dataList"></param>
        /// <param name="onRefreshItem"></param>
        /// <param name="onClickItem"></param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        /// <typeparam name="T"></typeparam>
        public static async ETTask InitAsync<T>(this GList list, List<T> dataList, Func<int, T, GObject, ETTask> onRefreshItem, Func<int, T, GObject, ETTask> onClickItem = null, int itemNums = -1,
        int selectedIndex = -1)
        {
            if (dataList == null || dataList.Count == 0)
            {
                list.numItems = 0;
                return;
            }
            
            list.itemRenderer = (async (index, obj) => { await onRefreshItem.Invoke(index, dataList[index], obj); });

            if (onClickItem != null)
            {
                list.onClickItem.Set(async (content) =>
                {
                    GObject item = content.data as GObject;
                    int childIndex = list.GetChildIndex(item);
                    await onClickItem.Invoke(list.ChildIndexToItemIndex(childIndex), dataList[list.ChildIndexToItemIndex(childIndex)], item);
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
                    list.selectedIndex = selectedIndex;
                    await onClickItem.Invoke(selectedIndex, dataList[selectedIndex], item);
                }
            }
        }
        
        #endregion

        #region GTree AddListner
        
        /// <summary>
        /// 初始化GTree（仅限2层结构：即只有根和叶）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onRefreshItem">刷新文件夹和页签回调</param>
        /// <param name="onClickItem">点击页签回调</param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="multiSelected">true：文件夹可多选 false：文件夹单选</param>
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

            // 刷新第一个节点
            onRefreshItem.Invoke(0, list.rootNode.GetChildAt(0));
        }
        
        /// <summary>
        /// 初始化TreeList（仅限2层结构：即只有根和叶）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="data">数据列表</param>
        /// <param name="onRefreshItem">刷新文件夹和页签回调</param>
        /// <param name="onClickItem">点击页签回调</param>
        /// <param name="selectIndex"></param>
        /// <param name="mulSelect">true：文件夹可多选 false：文件夹单选</param>
        /// <typeparam name="T"></typeparam>
        public static void InitTree<T>(this GTree list, List<T> data, Action<T, GTreeNode> onRefreshItem, Action<T, GTreeNode> onClickItem,
        int selectIndex = -1, bool mulSelect = false)
        {
            list.treeNodeRender = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(data[index], treeNode);
            };

            list.treeNodeWillExpand = (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                onRefreshItem.Invoke(data[index], treeNode);

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
                    onRefreshItem.Invoke(data[i + startIndex], item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        onClickItem.Invoke(data[i + startIndex], item);
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
                    if (!mulSelect)
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

                    //选中文件夹下第一个页签
                    onClickItem.Invoke(data[index], node);
                }
            };

            list.onClickItem.Set(listener);

            if (data.Count > 0 && selectIndex > -1)
            {
                GTreeNode treeNode = list.rootNode.GetChildAt(selectIndex);

                //展开文件夹
                list.treeNodeWillExpand(treeNode, true);
                //触发点击文件夹事件
                listener.Invoke(new EventContext() { data = treeNode.cell });
            }

            // 刷新第一个节点
            onRefreshItem.Invoke(data[0], list.rootNode.GetChildAt(0));
        }
        
        /// <summary>
        /// 异步初始化GTree（仅限2层结构：即只有根和叶）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="onRefreshItem">刷新文件夹和页签回调</param>
        /// <param name="onClickItem">点击页签回调</param>
        /// <param name="itemNums"></param>
        /// <param name="selectedIndex"></param>
        /// <param name="multiSelected">true：文件夹可多选 false：文件夹单选</param>
        public static async ETTask InitTreeAsync(this GTree list, Func<int, GTreeNode, ETTask> onRefreshItem, Func<int, GObject, ETTask> onClickItem, int itemNums = -1,
        int selectedIndex = -1, bool multiSelected = false)
        {
            list.treeNodeRender = async (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                await onRefreshItem.Invoke(index, treeNode);
            };

            list.treeNodeWillExpand = async (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                await onRefreshItem.Invoke(index, treeNode);

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
                    await onRefreshItem.Invoke(i + startIndex, item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        await onClickItem.Invoke(i + startIndex, item.cell);
                    }
                }
            };

            EventCallback1 listener = async (context) =>
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
                    await onClickItem.Invoke(index, item);
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

            // 刷新第一个节点
            await onRefreshItem.Invoke(0, list.rootNode.GetChildAt(0));
        }
        
        /// <summary>
        /// 异步初始化TreeList（仅限2层结构：即只有根和叶）
        /// </summary>
        /// <param name="list"></param>
        /// <param name="data">数据列表</param>
        /// <param name="onRefreshItem">刷新文件夹和页签回调</param>
        /// <param name="onClickItem">点击页签回调</param>
        /// <param name="selectIndex"></param>
        /// <param name="mulSelect">true：文件夹可多选 false：文件夹单选</param>
        /// <typeparam name="T"></typeparam>
        public static async ETTask InitTreeAsync<T>(this GTree list, List<T> data, Func<T, GTreeNode, ETTask> onRefreshItem, Func<T, GTreeNode, ETTask> onClickItem,
        int selectIndex = -1, bool mulSelect = false)
        {
            list.treeNodeRender = async (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                await onRefreshItem.Invoke(data[index], treeNode);
            };

            list.treeNodeWillExpand = async (treeNode, gcom) =>
            {
                int index = list.rootNode.GetChildIndex(treeNode);
                await onRefreshItem.Invoke(data[index], treeNode);

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
                    await onRefreshItem.Invoke(data[i + startIndex], item);

                    if (i == 0 && treeNode.expanded)
                    {
                        //点击文件夹下第一个页签
                        item.cell.asButton.selected = true;
                        await onClickItem.Invoke(data[i + startIndex], item);
                    }
                }
            };

            EventCallback1 listener = async (context) =>
            {
                GObject item = (GObject)context.data;

                int childIndex = list.GetChildIndex(item);
                int index = list.ChildIndexToItemIndex(childIndex);

                GTreeNode node = item.treeNode;
                if (node.isFolder)
                {
                    //单选时, 点击已经展开的文件夹,不会收起它
                    if (!mulSelect)
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

                    //选中文件夹下第一个页签
                    await onClickItem.Invoke(data[index], node);
                }
            };

            list.onClickItem.Set(listener);

            if (data.Count > 0 && selectIndex > -1)
            {
                GTreeNode treeNode = list.rootNode.GetChildAt(selectIndex);

                //展开文件夹
                list.treeNodeWillExpand(treeNode, true);
                //触发点击文件夹事件
                listener.Invoke(new EventContext() { data = treeNode.cell });
            }

            // 刷新第一个节点
            await onRefreshItem.Invoke(data[0], list.rootNode.GetChildAt(0));
        }
        
        #endregion
    }
}