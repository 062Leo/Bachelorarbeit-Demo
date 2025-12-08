import os
import codecs
from pathlib import Path

def convert_to_utf8_with_bom(directory="."):
    # Convert to Path object for better path handling
    directory = Path(directory).resolve()
    print(f"Starting conversion in directory: {directory}")
    
    # Counter for converted files
    converted_count = 0
    error_count = 0
    
    # Walk through all files in directory and subdirectories
    for root, _, files in os.walk(directory):
        for filename in files:
            if filename.lower().endswith('.cs'):
                file_path = Path(root) / filename
                try:
                    # Read the file with error handling for different encodings
                    with open(file_path, 'rb') as file:
                        content = file.read()
                    
                    # Try to decode with UTF-8 first, then fall back to windows-1252
                    try:
                        content = content.decode('utf-8-sig')
                    except UnicodeDecodeError:
                        try:
                            content = content.decode('windows-1252')
                            print(f"Converted from windows-1252: {file_path}")
                        except Exception as e:
                            print(f"Error decoding {file_path}: {e}")
                            error_count += 1
                            continue
                    
                    # Write back with UTF-8 with BOM
                    with codecs.open(file_path, 'w', 'utf-8-sig') as file:
                        file.write(content)
                    
                    converted_count += 1
                    print(f"Converted: {file_path}")
                    
                except Exception as e:
                    print(f"Error processing {file_path}: {e}")
                    error_count += 1
    
    print(f"\nConversion complete!")
    print(f"Files converted: {converted_count}")
    print(f"Errors: {error_count}")

if __name__ == "__main__":
    # Get the directory where the script is located
    script_dir = os.path.dirname(os.path.abspath(__file__))
    convert_to_utf8_with_bom(script_dir)