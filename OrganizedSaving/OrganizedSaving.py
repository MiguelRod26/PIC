## In a folder with images from a protocol and said protocol call this func to organize the images into Stims folders with before and after folders
## The images need to be saved as the camera frame in which they were taken. (This doesnt mean all frames have to be accounted for)
##Run in terminal: "python script_name.py /path/to/directory -r n" to use the filter_files func and without -r it will assume the movefiles func

import os
import shutil
import sys


def create_nth_line_file(stim_folder_path, file_path, n):
    # Check if the specified file exists in the folder
    if not os.path.isfile(file_path):
        print("File not found in folder.")
        return
    
    # Read the original file and extract the nth line
    with open(file_path, 'r') as f:
        lines = f.readlines()
        non_comment_lines = [line for line in lines if not line.startswith('#')]
        nth_non_comment_line = non_comment_lines[n-1]


    # Create a new file with the nth line
    new_file_path = os.path.join(stim_folder_path, "TrialInfo.txt")
    with open(new_file_path, 'w') as TrialInfo:
        TrialInfo.write(nth_non_comment_line)

    return new_file_path

def IntervalGetter(file_path):
    # Check if the specified file exists in the folder
    if not os.path.isfile(file_path):
        print("File not found in folder.")
        return
    
    # Read the original file and extract the first non-comment line
    with open(file_path, 'r') as f:
        lines = f.readlines()
        non_comment_lines = [line for line in lines if not line.startswith('#')]
        first_non_comment_line = non_comment_lines[0]

    # Extract the first two numbers from the line
    Intervals = []
    for char in first_non_comment_line:
        if char.isnumeric():
            if not Intervals or type(Intervals[-1]) == str:
                Intervals.append(int(char))
            else:
                Intervals[-1] = Intervals[-1] * 10 + int(char)

    return Intervals


##If you want the 2n images around the stimulus use this func with range as n
def filter_files(drive_letter, range):
    i = 1
    m = 0
    while True:
        # Define the main folder path
        folder_path = os.path.join(drive_letter, "folder", "subfolder")
        stim_folder_path = os.path.join(folder_path, "Stim1")

        #Define the ranges for the images in each folder
        rangebefore = [Interval[0]+(i-1)*Interval[1]//2 - range,Interval[0]+(i-1)*Interval[1]//2]
        rangeafter = [Interval[0]+(i-1)*Interval[1]//2, Interval[0]+(i-1)*Interval[1]//2 + range]

        # Check if the folder exists, create it if not
        while os.path.exists(stim_folder_path):
            i += 1
            stim_folder_path = os.path.join(folder_path, "Stim" + str(i))
        os.mkdir(stim_folder_path)

        # Define the "b" and "a" subfolders within the main folder and check if the before and after subfolders exist, create them if not
        before_folder_path = os.path.join(stim_folder_path, "Before")
        after_folder_path = os.path.join(stim_folder_path, "After")
        notused_folder_path = os.path.join(stim_folder_path, "NotUsed")

        if not os.path.isdir(before_folder_path):
            os.mkdir(before_folder_path)
        if not os.path.isdir(after_folder_path):
            os.mkdir(after_folder_path)
        if not os.path.isdir(after_folder_path):
            os.mkdir(notused_folder_path)   

        filepath = os.path.join(folder_path, "file.txt")
        Interval = IntervalGetter(filepath)

        # Loop through all the files in the folder
        for file in os.listdir(folder_path):

            filepath = os.path.join(folder_path, file)

            if os.path.isfile(filepath):

                # Check if the file is a text file
                if file.endswith(".txt"):
                    new_file_path = create_nth_line_file(stim_folder_path, filepath, n=i+2)
                    # Move the new file to the stim folder
                    shutil.move(new_file_path, os.path.join(stim_folder_path, os.path.basename(new_file_path)))

                else:

                    if m in rangebefore:
                        # Move the file to the before subfolder
                        shutil.move(filepath, os.path.join(before_folder_path, file))
                        m += 1
                    if m in rangeafter:
                        # Move the file to the after subfolder
                        shutil.move(filepath, os.path.join(after_folder_path, file))
                        m += 1
                    if m == Interval[0]+i*Interval[1]//2:
                        break
                    else:
                        shutil.move(filepath, os.path.join(notused_folder_path, file))
                        m += 1

        # Check if there are any files left in the folder
        files_left = False
        for file in os.listdir(folder_path):
            if os.path.isfile(os.path.join(folder_path, file)):
                files_left = True
                break

        if not files_left:
            # All files are organized
            break

    pass


##If you want to preserve all of the files use this func
def move_files(folder_path):
    i=1
    m = 0
    while True:
        # Define the main folder path
        stim_folder_path = os.path.join(folder_path, "Stim1")
        

        # Check if the folder exists, create it if not
        while os.path.exists(stim_folder_path):
            i += 1
            stim_folder_path = os.path.join(folder_path, "Stim" + str(i))
        os.mkdir(stim_folder_path)

        # Define the "b" and "a" subfolders within the main folder and check if the before and after subfolders exist, create them if not
        before_folder_path = os.path.join(stim_folder_path, "Before")
        after_folder_path = os.path.join(stim_folder_path, "After")
 
        if not os.path.isdir(before_folder_path):
            os.mkdir(before_folder_path)
        if not os.path.isdir(after_folder_path):
            os.mkdir(after_folder_path)
    

        Interval = IntervalGetter(filepath)
        
        # Loop through all the files in the folder
        for file in os.listdir(folder_path):

            filepath = os.path.join(folder_path, file)

            if os.path.isfile(filepath):

                # Check if the file is a text file
                if file.endswith(".txt"):

                    new_file_path = create_nth_line_file(folder_path, filepath, n=i+2)
                    # Move the new file to the main folder
                    shutil.move(new_file_path, os.path.join(stim_folder_path, os.path.basename(new_file_path)))

                else:

                    if m< Interval[0]+(i-1)*Interval[1]//2:
                        # Move the file to the before subfolder
                        shutil.move(filepath, os.path.join(before_folder_path, file))
                        m += 1
                    else:
                        # Move the file to the after subfolder
                        shutil.move(filepath, os.path.join(after_folder_path, file))
                        m += 1
                        if m == Interval[0]+i*Interval[1]//2:
                            break

        # Check if there are any files left in the folder
        files_left = False
        for file in os.listdir(folder_path):
            if os.path.isfile(os.path.join(folder_path, file)):
                files_left = True
                break
        
        if not files_left:
            # All files are organized
            break
    pass

def main():
    if len(sys.argv) < 2:
        print("Please specify the directory path.")
        sys.exit(1)

    directory = sys.argv[1]

    if not os.path.isdir(directory):
        print(f"{directory} is not a valid directory path.")
        sys.exit(1)

    # Read file path from command line arguments
    file_path = sys.argv[1]

    # Check if optional flag "-r" is present with an integer value
    range_param = None
    if len(sys.argv) >= 3 and sys.argv[2] == "-r":
        try:
            range_param = int(sys.argv[3])
        except ValueError:
            range_param = 100
            pass

    # Get list of files in directory
    #files = os.listdir(file_path)

    # Call appropriate function based on presence of range parameter
    if range_param is not None:
        filter_files(file_path, range_param)
    else:
        move_files(file_path)

if __name__ == "__main__":
    main()

        