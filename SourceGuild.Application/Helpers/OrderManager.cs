namespace SourceGuild.Application.Helpers;

public static class OrderManager
{
    // Shifts order for insert: increments order of all items >= newOrder
    public static void ShiftOrderForInsert<T>(IList<T> items, int newOrder, Func<T, int> getOrder, Action<T, int> setOrder, Action<T>? touch = null)
    {
        foreach (var item in items.Where(i => getOrder(i) >= newOrder).OrderBy(i => getOrder(i)))
        {
            setOrder(item, getOrder(item) + 1);
            touch?.Invoke(item);
        }
    }

    // Shifts order for update: moves item from oldOrder to newOrder
    public static void ShiftOrderForUpdate<T>(IList<T> items, int oldOrder, int newOrder, Func<T, int> getOrder, Action<T, int> setOrder, Action<T>? touch = null)
    {
        if (newOrder > oldOrder)
        {
            foreach (var item in items.Where(i => getOrder(i) > oldOrder && getOrder(i) <= newOrder).OrderByDescending(i => getOrder(i)))
            {
                setOrder(item, getOrder(item) - 1);
                touch?.Invoke(item);
            }
        }
        else if (newOrder < oldOrder)
        {
            foreach (var item in items.Where(i => getOrder(i) >= newOrder && getOrder(i) < oldOrder).OrderBy(i => getOrder(i)))
            {
                setOrder(item, getOrder(item) + 1);
                touch?.Invoke(item);
            }
        }
    }

    // Shifts order for delete: decrements order of all items > deletedOrder
    public static void ShiftOrderForDelete<T>(IList<T> items, int deletedOrder, Func<T, int> getOrder, Action<T, int> setOrder, Action<T>? touch = null)
    {
        foreach (var item in items.Where(i => getOrder(i) > deletedOrder).OrderBy(i => getOrder(i)))
        {
            setOrder(item, getOrder(item) - 1);
            touch?.Invoke(item);
        }
    }
}
