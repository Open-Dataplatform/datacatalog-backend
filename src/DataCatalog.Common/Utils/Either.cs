using System;

namespace DataCatalog.Common.Utils
{
    public class Either<TLeft, TRight>
    {
        private readonly TLeft _left;
        private readonly TRight _right;
        private readonly bool _isLeft;

        public Either(TLeft left)
        {
            _left = left;
            _isLeft = true;
        }

        public Either(TRight right)
        {
            _right = right;
        }

        public T Match<T>(Func<TLeft, T> matchLeft, Func<TRight, T> matchRight)
        {
            return _isLeft ? matchLeft.Invoke(_left) : matchRight.Invoke(_right);
        }
    }
}