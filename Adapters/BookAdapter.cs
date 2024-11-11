using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.RecyclerView.Widget;
using com.companyname.navigationgraph9net9.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.companyname.navigationgraph9net9.Adapters
{
    internal class BookAdapter : RecyclerView.Adapter
    {
        private List<Book> books;
        
        public BookAdapter(List<Book> books)
        {
            this.books = books;
        }

        public override int ItemCount => books.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is BookViewHolder bookViewHolder)
            {
                Book book = books[position];
                bookViewHolder.TitleTextView!.Text = book.Title;
                bookViewHolder.AuthorTextView!.Text = book.Author;
                bookViewHolder.ReleaseDateTextView!.Text = book.ReleaseDate;
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View? itemView = LayoutInflater.From(parent.Context)!.Inflate(Resource.Layout.book_item, parent, false);
            return new BookViewHolder(itemView!);
        }

        private class BookViewHolder : RecyclerView.ViewHolder
        {
            public TextView? TitleTextView { get; }
            public TextView? AuthorTextView { get; }
            public TextView? ReleaseDateTextView { get; }


            public BookViewHolder(View itemView) : base(itemView)
            {
                TitleTextView = itemView.FindViewById<TextView>(Resource.Id.book_title_textview);
                AuthorTextView = itemView.FindViewById<TextView>(Resource.Id.book_author_textview);
                ReleaseDateTextView = itemView.FindViewById<TextView>(Resource.Id.book_publishdate_textview);
            }
        }

        public void UpdateBooks(List<Book> newBooks)
        {
            books = newBooks;
            NotifyDataSetChanged();
        }

    }
}
