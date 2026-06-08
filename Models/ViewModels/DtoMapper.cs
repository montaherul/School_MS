namespace SchoolManagementSystem.Models.ViewModels;

internal static class DtoMapper
{
    internal static TTarget MapTo<TTarget>(this object source)
        where TTarget : new()
    {
        var target = new TTarget();
        var sourceProps = source.GetType().GetProperties();
        var targetProps = typeof(TTarget).GetProperties();
        foreach (var sp in sourceProps)
        {
            if (!sp.CanRead) continue;
            var tp = Array.Find(targetProps, p =>
                p.Name == sp.Name &&
                p.PropertyType == sp.PropertyType &&
                p.CanWrite);
            if (tp != null)
                tp.SetValue(target, sp.GetValue(source));
        }
        return target;
    }
}
