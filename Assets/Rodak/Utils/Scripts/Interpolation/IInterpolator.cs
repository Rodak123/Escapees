namespace Rodak.Animation.Interpolation
{
    public interface IInterpolator<T>
    {
        public T GetValue(float t, T start, T end);
    }
}