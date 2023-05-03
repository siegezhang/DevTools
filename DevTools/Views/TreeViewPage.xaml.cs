// Copyright (c) Microsoft Corporation and Contributors.
// Licensed under the MIT License.

using System.Text.Json;
using System.Text.Json.Nodes;
using DevTools.Core.Models;
using DevTools.ViewModels;
using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.
namespace DevTools.Views;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class TreeViewPage : Page
{

    public TreeViewModel ViewModel
    {
        get;
    }

    public TreeViewPage()
    {
        ViewModel = App.GetService<TreeViewModel>();
        this.InitializeComponent();
    }

    private void IterateJsonArray(JsonArray jsonArray, TreeViewNode treeViewNode)
    {
        foreach (var e in jsonArray)
        {
            if (e is JsonObject)
            {
                IterateJsonObject(e.AsObject(), treeViewNode);
            }
            else if (e is JsonArray)
            {
                IterateJsonArray(e.AsArray(), treeViewNode);
            }
            else
            {
                TreeViewNode t = new TreeViewNode() { Content = e.ToString() };
                treeViewNode.Children.Add(t);
            }
        }
    }

    private void IterateJsonObject(JsonObject jsonObject, TreeViewNode treeViewNode)
    {
        foreach (var e in jsonObject)
        {
            var valueObj = e.Value;
            if (valueObj is JsonArray)
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key };
                treeViewNode.Children.Add(t);
                if (valueObj.AsArray().Count == 0)
                {
                    t.Content = e.Key + ":[]";
                    continue;
                }

                IterateJsonArray(valueObj.AsArray(), t);
            }
            else if (valueObj is JsonObject)
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key };
                treeViewNode.Children.Add(t);
                IterateJsonObject(valueObj.AsObject(), t);
            }
            else
            {
                TreeViewNode t = new TreeViewNode() { Content = e.Key + ":" + valueObj };
                treeViewNode.Children.Add(t);
            }
        }
    }

    private void TabView_AddTabButtonClick(TabView sender, object args)
    {
        var newTab = new TabViewItem();
        newTab.IconSource = new SymbolIconSource() { Symbol = Symbol.Document };
        newTab.Header = "New Document";
        // The Content of a TabViewItem is often a frame which hosts a page.
        // Frame frame = new Frame();
        // newTab.Content = frame;
        // frame.Navigate(typeof(TreeViewPage));

        sender.TabItems.Add(newTab);
    }

    private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
    {
        sender.TabItems.Remove(args.Tab);
    }

    private void TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel.MessageInfos = new MessageInfo();
        if (sender == null)
        {
            return;
        }

        treeView.RootNodes.Clear();
        string jsonText = (sender as TextBox).Text;
        Console.WriteLine($"jsonText:{jsonText}");
        if (jsonText.Equals(String.Empty))
        {
            return;
        }

        JsonNode node;
        try
        {
             node = JsonNode.Parse(jsonText);
        }
        catch (JsonException exception)
        {
            ViewModel.MessageInfos = new MessageInfo()
            {
                IsOpen = true,
                Message = exception.ToString()
            };
            Console.WriteLine(exception);
            //throw;
            return;
        }
      
        TreeViewNode myTreeViewNode = new TreeViewNode() { Content = "JSON" };
        if (node is JsonArray)
        {
            JsonArray jsonObject = node.AsArray();
            IterateJsonArray(jsonObject, myTreeViewNode);
        }
        else if (node is JsonObject)
        {
            JsonObject jsonObject = node.AsObject();
            IterateJsonObject(jsonObject, myTreeViewNode);
        }

        treeView.RootNodes.Add(myTreeViewNode);
    }
}