using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;

namespace CandyKeeper.Presentation.Extensions;

public class ListBoxExtensions
{
    public static readonly DependencyProperty SelectedItemsProperty = 
        DependencyProperty.RegisterAttached(
            "SelectedItems",
            typeof(IList),
            typeof(ListBoxExtensions),
            new PropertyMetadata(null, OnSelectedItemsChanged));

     public static void SetSelectedItems(DependencyObject element, IList value)
        {
            element.SetValue(SelectedItemsProperty, value);
        }

        public static IList GetSelectedItems(DependencyObject element)
        {
            return (IList)element.GetValue(SelectedItemsProperty);
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListBox listBox)
            {
                if (e.OldValue is INotifyCollectionChanged oldValue)
                {
                    oldValue.CollectionChanged -= OnCollectionChanged;
                }

                if (e.NewValue is INotifyCollectionChanged newValue)
                {
                    listBox.SelectedItems.Clear();
                    foreach (var item in (IEnumerable)e.NewValue)
                    {
                        listBox.SelectedItems.Add(item);
                    }
                    newValue.CollectionChanged += OnCollectionChanged;
                }
            }
        }

        private static void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (sender is ListBox listBox)
            {
                var selectedItems = GetSelectedItems(listBox);
                if (selectedItems == null) return;

                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var newItem in e.NewItems)
                        {
                            if (!listBox.SelectedItems.Contains(newItem))
                            {
                                listBox.SelectedItems.Add(newItem);
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var oldItem in e.OldItems)
                        {
                            if (listBox.SelectedItems.Contains(oldItem))
                            {
                                listBox.SelectedItems.Remove(oldItem);
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        listBox.SelectedItems.Clear();
                        break;
                }
            }
        }
}