from music21 import *
import sys
import os
import subprocess

def convert_musescore_to_text(file_path):
    print(f"Starting conversion of: {file_path}")
    
    musescore_path = r'C:\Program Files\MuseScore 4\bin\MuseScore4.exe'
    temp_xml = file_path.rsplit('.', 1)[0] + '.xml'
    
    try:
        subprocess.run([musescore_path, file_path, '-o', temp_xml], check=True)
    except subprocess.CalledProcessError as e:
        print(f"Error running MuseScore: {e}")
        raise
    
    if not os.path.exists(temp_xml):
        raise Exception(f"Failed to create XML file at: {temp_xml}")
    
    score = converter.parse(temp_xml)
    parts = score.parts
    part = parts[0]
    
    # Debug information
    print(f"\nScore information:")
    print(f"Number of parts: {len(parts)}")
    print(f"Part name: {part.partName}")
    
    # Get total number of measures using measures property
    measures = list(part.getElementsByClass('Measure'))
    total_measures = len(measures)
    print(f"Total measures in score: {total_measures}")
    
    # Verify first measure content
    if measures:
        first_measure = measures[0]
        print(f"\nFirst measure contents:")
        for element in first_measure.recurse():
            if isinstance(element, (note.Note, chord.Chord)):
                print(f"Found: {element}")
    
    def convert_to_sharp(note_str):
        replacements = {
            'B-': 'A#', 'E-': 'D#', 'A-': 'G#', 'D-': 'C#', 'G-': 'F#', 'C-': 'B'
        }
        for flat, sharp in replacements.items():
            note_str = note_str.replace(flat, sharp)
        return note_str
    
    def adjust_octave(note_str):
        note_name = ''.join(c for c in note_str if not c.isdigit())
        octave = int(''.join(c for c in note_str if c.isdigit()))
        
        while octave > 4 or (octave == 4 and note_name in ['A', 'A#', 'B']):
            octave -= 1
            
        while octave < 1:
            octave += 1
            
        return f"{note_name}{octave}"
    
    # Get all notes and their measure numbers
    notes_by_measure = {}
    
    # Process each measure directly
    for measure in measures:
        measure_number = measure.number
        notes_by_measure[measure_number] = []
        
        # Dictionary to store notes by their offset (timing)
        notes_by_offset = {}
        
        # First pass: collect all notes and their timings
        for element in measure.recurse():
            if isinstance(element, (note.Note, chord.Chord)):
                offset = element.offset  # Get the timing position
                if offset not in notes_by_offset:
                    notes_by_offset[offset] = []
                
                if isinstance(element, note.Note):
                    note_text = convert_to_sharp(element.nameWithOctave)
                    note_text = adjust_octave(note_text)
                    notes_by_offset[offset].append(note_text)
                else:  # Chord
                    for n in element:
                        note_text = convert_to_sharp(n.nameWithOctave)
                        note_text = adjust_octave(note_text)
                        notes_by_offset[offset].append(note_text)
        
        # Second pass: process notes by timing
        for offset in sorted(notes_by_offset.keys()):
            notes = notes_by_offset[offset]
            if len(notes) > 1:
                # Multiple notes at same timing - make it a chord/double stop
                chord_text = '+'.join(sorted(set(notes)))  # Using set to remove duplicates
                notes_by_measure[measure_number].append(chord_text)
            else:
                # Single note
                notes_by_measure[measure_number].append(notes[0])
    
    # Convert to result
    result = []
    print("\nMeasure processing details:")
    for measure_num in range(1, total_measures + 1):
        if measure_num in notes_by_measure:
            notes = notes_by_measure[measure_num]
            if notes:  # Only add measures that contain notes
                result.append(' '.join(notes))
                print(f"Measure {measure_num}: {' '.join(notes)}")
            else:
                print(f"Measure {measure_num}: Empty measure (skipped)")
        else:
            print(f"Measure {measure_num}: No measure data found (skipped)")
    
    # Clean up
    try:
        os.remove(temp_xml)
    except Exception as e:
        print(f"Warning: Could not remove temporary file: {e}")
        
    return '\n'.join(result)

if __name__ == "__main__":
    if len(sys.argv) != 2:
        print("Usage: python PythonTransformator.py <musescore_file>")
        sys.exit(1)
        
    musescore_file = sys.argv[1]
    try:
        result = convert_musescore_to_text(musescore_file)
        print("\nTotal lines in output:", len(result.splitlines()))
        
        output_file = musescore_file.rsplit('.', 1)[0] + '_converted.txt'
        with open(output_file, 'w') as f:
            f.write(result)
        print(f"\nConverted text saved to: {output_file}")
        
    except Exception as e:
        print(f"Error: {e}")

