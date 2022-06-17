using System;
using System.Collections.Generic;
using System.Linq;

namespace amethyst_installer_gui {
    public static class LinqExtensions {
        public static IEnumerable<T> Return<T>(T element) {
            yield return element;
        }

        public static IEnumerable<T> StartWith<T>(
            this IEnumerable<T> list, T element) {
            return Return(element).Concat(list);
        }

        public static IEnumerable<TEntity> Flatten<TEntity>(
            this TEntity element,
            Func<TEntity, IEnumerable<TEntity>> childSelector) {
            if ( childSelector(element) != null )
                return childSelector(element)
                    .SelectMany(child => child.Flatten(childSelector))
                    .StartWith(element);

            var items = new List<TEntity> { element };
            return items;
        }
    }
}
