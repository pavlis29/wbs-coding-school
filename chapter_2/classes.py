class Book:
    def __init__(self, name, author, release_date):
        self.name = name
        self.author = author
        self.release_date = release_date
        self.is_read = False


class BookCollection:
    def __init__(self, list_of_books):
        self.list_of_books = []
        if len(list_of_books) > 0:
            for book in list_of_books:
                if not isinstance(book, Book):
                    raise ValueError("Must be instance of a Book")
                else:
                    self.list_of_books.append(book)

    def add_book(self, book):
        if not isinstance(book, Book):
            raise ValueError("Must be instance of a Book")
        else:
            self.list_of_books.append(book)

    def mark_as_read(self, book_name):
        for book in self.list_of_books:
            if book.name == book_name:
                book.is_read = True
                print("Book found!")
                break
        else:
            print("Book not in the collection.")

    def list_books(self):
        for book in self.list_of_books:
            print(
                f"{book.name} from {book.author} release at: {book.release_date}. Read status: {book.is_read}"
            )


book1 = Book("Book1", "Author1", 1991)
book2 = Book("Book2", "Author2", 1896)
book3 = Book("Book3", "Author3", 1955)
book4 = Book("Book4", "Author4", 1955)

my_collection = BookCollection([book1, book2, book3])

my_collection.list_books()
print()
my_collection.add_book(book4)

my_collection.list_books()

my_collection.mark_as_read("Book2")

print()

my_collection.list_books()
