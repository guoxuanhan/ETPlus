/** This is an automatically generated class by FUICodeSpawner. Please do not modify it. **/

using System.Collections.Generic;
using FairyGUI.Dynamic;

namespace ET.Client
{
    [EnableClass]
    public sealed class UIPackageMapping : IUIPackageHelper
    {
        private readonly Dictionary<string, string> m_PackageIdToNameMap = new()
        {
            {"f2boiu4i", "Common"},
            {"5qlx9ljj", "Example1Pkg"},
            {"73sapar1", "ExampleListPkg"},
            {"2f8jqeff", "HotUpdate"},
            {"9gkqq49y", "Icon1"},
            {"96tfczmn", "Icon2"},
            {"9cdyueiu", "Icon3"},
            {"9bg9r3vf", "Icon4"},
            {"ti3ka994", "Lobby"},
            {"qptb9pl1", "LoginPkg"},
            {"2kcjlx6n", "TestA"},
            {"296l7tjh", "TestB"},
            // <last line>
        };

        public string GetPackageNameById(string id)
        {
            return m_PackageIdToNameMap.TryGetValue(id, out var packageName) ? packageName : null;
        }
    }
}
