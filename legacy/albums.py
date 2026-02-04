#!/usr/bin/env python3
"""
Album Month Statistics Program
Translated from COBOL to Python
Author: Michael Coughlan (Original COBOL)

This program reads album data from a file and counts how many albums
were released in each month of the year.
"""

from typing import List, Optional
import os


class AlbumDetails:
    """Data structure representing an album record"""
    
    def __init__(self, album_id: int, artist: str, title: str,
                 year_of_release: int, month_of_release: int,
                 day_of_release: int, genre: str):
        self.album_id = album_id
        self.artist = artist
        self.title = title
        self.year_of_release = year_of_release
        self.month_of_release = month_of_release
        self.day_of_release = day_of_release
        self.genre = genre


class AlbumMonthCounter:
    """Counts albums by release month"""
    
    MONTHS = [
        "January", "February", "March", "April",
        "May", "June", "July", "August",
        "September", "October", "November", "December"
    ]
    
    def __init__(self, filename: str = "ALBUMS.DAT"):
        self.filename = filename
        self.month_counts = [0] * 12  # Initialize counts for 12 months
        
    def parse_album_record(self, line: str) -> Optional[AlbumDetails]:
        """
        Parse a fixed-width album record from the file.
        
        Record layout:
        - AlbumId: 7 digits (positions 0-6)
        - Artist: 8 characters (positions 7-14)
        - Title: 20 characters (positions 15-34)
        - Year: 4 digits (positions 35-38)
        - Month: 2 digits (positions 39-40)
        - Day: 2 digits (positions 41-42)
        - Genre: 10 characters (positions 43-52)
        """
        if not line or len(line) < 43:
            return None
            
        try:
            album_id = int(line[0:7].strip())
            artist = line[7:15].strip()
            title = line[15:35].strip()
            year = int(line[35:39].strip())
            month = int(line[39:41].strip())
            day = int(line[41:43].strip())
            genre = line[43:53].strip() if len(line) >= 53 else ""
            
            return AlbumDetails(
                album_id=album_id,
                artist=artist,
                title=title,
                year_of_release=year,
                month_of_release=month,
                day_of_release=day,
                genre=genre
            )
        except (ValueError, IndexError):
            return None
    
    def read_and_count_albums(self) -> None:
        """Read album file and count albums by month"""
        if not os.path.exists(self.filename):
            print(f"Error: File '{self.filename}' not found.")
            return
        
        try:
            with open(self.filename, 'r', encoding='utf-8') as album_file:
                for line in album_file:
                    album = self.parse_album_record(line)
                    
                    if album and 1 <= album.month_of_release <= 12:
                        # Convert 1-based month to 0-based index
                        month_idx = album.month_of_release - 1
                        self.month_counts[month_idx] += 1
                        
        except IOError as e:
            print(f"Error reading file: {e}")
            return
    
    def display_results(self) -> None:
        """Display the monthly album counts"""
        print(" Month       AlbumCount")
        print("-" * 25)
        
        for month_idx in range(12):
            month_name = self.MONTHS[month_idx]
            count = self.month_counts[month_idx]
            print(f"{month_name:<12} {count:>3}")
    
    def run(self) -> None:
        """Main program execution"""
        self.read_and_count_albums()
        self.display_results()


def main():
    """Program entry point"""
    counter = AlbumMonthCounter("ALBUMS.DAT")
    counter.run()


if __name__ == "__main__":
    main()
