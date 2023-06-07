## In a folder with images from a protocol and said protocol call this func to organize the images into Stims folders with before and after folders
## The images need to be saved as the camera frame in which they were taken. (This doesnt mean all frames have to be accounted for)

##Run in terminal: "python script_name.py /path/to/directory -r n" to use the filter_files func and without -r it will assume the movefiles func
##Problem solving: If the path has spaces in it it will mess with the argument counting so comment the Main method in main and use the brute method
##Note: It creates an extra stim so it can store the last images in the before folder. CBA to solve it 
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
    intervals = []

    with open(file_path, 'r') as file:
        for line in file:
            line = line.strip()
            if not line.startswith('#'):
                numbers = line.split(',')
                if len(numbers) == 2:
                    try:
                        num1 = float(numbers[0].strip())
                        num2 = float(numbers[1].strip())
                        intervals = [num1, num2]
                        break  # Stop reading after the first valid line
                    except ValueError:
                        pass

    return intervals


##If you want the 2n images around the stimulus use this func with range as n
def filter_files(folder_path, range_val):
    i = 1
    m = 0

    while True:
        # Define the main folder path
        stim_folder_path = os.path.join(folder_path, "Stim1")

        # Check if the folder exists, create it if not
        while os.path.exists(stim_folder_path):
            i += 1
            stim_folder_path = os.path.join(folder_path, "Stim" + str(i))

        # Check if the folder exists, create it if not
        if not os.path.exists(stim_folder_path):
            os.makedirs(stim_folder_path)
       
        print(i)
        # Define the "b" and "a" subfolders within the main folder and check if the before and after subfolders exist, create them if not
        before_folder_path = os.path.join(stim_folder_path, "Before")
        after_folder_path = os.path.join(stim_folder_path, "After")
        notused_folder_path = os.path.join(folder_path, "NotUsed")

        if not os.path.isdir(before_folder_path):
            os.mkdir(before_folder_path)
        if not os.path.isdir(after_folder_path):
            os.mkdir(after_folder_path)
        if not os.path.isdir(notused_folder_path):
            os.mkdir(notused_folder_path)   

        

       

        # Loop through all the files in the folder to find the protocol and get the intervals and the stim settings
        for file in os.listdir(folder_path):
            filepath = os.path.join(folder_path, file)

            if os.path.isfile(filepath) and file.endswith(".txt"):
                Interval = IntervalGetter(filepath)
                #Define the ranges for the images in each folder  
                start_frame = Interval[0]+(i-1)*Interval[1]
                rangebefore = [start_frame - range_val,start_frame]
                rangeafter = [start_frame, start_frame + range_val]
                #Create new file
                new_file_path = create_nth_line_file(stim_folder_path, filepath, n=i+1)
                # Move the new file to the stim folder
                shutil.move(new_file_path, os.path.join(stim_folder_path, os.path.basename(new_file_path)))
        #print('Intervalscomsucesso')
        # Loop through all the files in the folder and divide them into subfolders
        for file in os.listdir(folder_path):
            
            filepath = os.path.join(folder_path, file)
            #print(filepath)
            if os.path.isfile(filepath):
                #print('INfor3')
                # Check if the file is a text file
                if file.endswith(".txt"):
                    pass

                else:
                    #print(m)
                    if rangebefore[0]<= m and m < rangebefore[1]:
                    # Move the file to the before subfolder
                        #print('inrangebefore')
                        shutil.move(filepath, os.path.join(before_folder_path, file))

                    elif rangeafter[0]<= m  and m <= rangeafter[1]:
                    # Move the file to the after subfolder
                       #print('inrangeafter')
                       shutil.move(filepath, os.path.join(after_folder_path, file))

                    elif m == rangeafter[1]+1:
                      break

                    else:
                     shutil.move(filepath, os.path.join(notused_folder_path, file))
                     
                    ##For python 3.10
                    # match m:
                    #     case rangebefore[0]<= m and m < rangebefore[1]:
                    #         # Move the file to the before subfolder
                    #         print('inrangebefore')
                    #         shutil.move(filepath, os.path.join(before_folder_path, file))

                    #     case rangeafter[0]<= m  and m <= rangeafter[1]:
                    #         # Move the file to the after subfolder
                    #         print('inrangeafter')
                    #         shutil.move(filepath, os.path.join(after_folder_path, file))

                    #     case m == Interval[0]+i*Interval[1]//2:
                    #         break
                    #     case _:
                    #         shutil.move(filepath, os.path.join(notused_folder_path, file))
                    m += 1

        #print('organizado stim folder')

        # Check if there are any non-.txt files or folders left in the folder
        files_left = any(os.path.isfile(os.path.join(folder_path, file)) and not file.endswith(".txt") for file in os.listdir(folder_path))

        if not files_left:
            # All remaining files are either .txt files or folders
            break



    pass


##If you want to preserve all of the files use this func
def move_files(folder_path):
    i = 1
    m = 0

    while True:
        # Define the main folder path
        stim_folder_path = os.path.join(folder_path, "Stim1")

        # Check if the folder exists, create it if not
        while os.path.exists(stim_folder_path):
            i += 1
            stim_folder_path = os.path.join(folder_path, "Stim" + str(i))

        # Check if the folder exists, create it if not
        if not os.path.exists(stim_folder_path):
            os.makedirs(stim_folder_path)
       
        #print(i)
        # Define the "b" and "a" subfolders within the main folder and check if the before and after subfolders exist, create them if not
        before_folder_path = os.path.join(stim_folder_path, "Before")
        after_folder_path = os.path.join(stim_folder_path, "After")

        if not os.path.isdir(before_folder_path):
            os.mkdir(before_folder_path)
        if not os.path.isdir(after_folder_path):
            os.mkdir(after_folder_path)

        

       

        # Loop through all the files in the folder to find the protocol and get the intervals and the stim settings
        for file in os.listdir(folder_path):
            filepath = os.path.join(folder_path, file)

            if os.path.isfile(filepath) and file.endswith(".txt"):
                Interval = IntervalGetter(filepath)                
                #Define the ranges for the images in each folder  
                start_frame = Interval[0]+(i-1)*Interval[1]

                #Create new file
                new_file_path = create_nth_line_file(stim_folder_path, filepath, n=i+1)

                # Move the new file to the stim folder
                shutil.move(new_file_path, os.path.join(stim_folder_path, os.path.basename(new_file_path)))
        
        # Loop through all the files in the folder and divide them into subfolders
        for file in os.listdir(folder_path):
            
            filepath = os.path.join(folder_path, file)

            if os.path.isfile(filepath):

                # Check if the file is a text file
                if file.endswith(".txt"):
                    pass

                else:
                    #print(m)
                    if m< start_frame:
                    # Move the file to the before subfolder                    
                        shutil.move(filepath, os.path.join(before_folder_path, file))

                    elif m == (Interval[0]+(i)*Interval[1] - Interval[1]/4):
                      break

                    else:
                     shutil.move(filepath, os.path.join(after_folder_path, file))
                    
                    m += 1

        #print('organizado stim folder')

        # Check if there are any files left in the folder, besides .txt files
        files_left = any(os.path.isfile(os.path.join(folder_path, file)) and not file.endswith(".txt") for file in os.listdir(folder_path))

        if not files_left:
        # All remaining files are either .txt files or folders
            break


    pass

def main():
    
    ######Brute method
    file_path = "/media/miguel/My Passport/Piezo_20_40_60_2minITI/piezo_190405/190405_GrRaised_fish2" #"media/miguel/My Passport/Piezo_20_40_60_2minITI/piezo_190405/190405_GrRaised_fish2 -test"
    range_param = 250
    filter_files(file_path, range_param)
    #move_files(file_path)
    print('Tudo arrumado')
    

    ######Main method
    #   if len(sys.argv) < 2:
    #       print("Please specify the directory path.")
    #       sys.exit(1)

    #   directory = sys.argv[1]

    #   if not os.path.isdir(directory):
    #       print(f"{directory} is not a valid directory path.")
    #       sys.exit(1)

    #   # Read file path from command line arguments
    #   file_path = sys.argv[1]

    #   # Check if optional flag "-r" is present with an integer value
    #   range_param = None
    #   if len(sys.argv) >= 3 and sys.argv[2] == "-r":
    #       try:
    #           range_param = int(sys.argv[3])
    #       except ValueError:
    #           range_param = 50
    #           pass


    #   # Call appropriate function based on presence of range parameter
    #   if range_param is not None:
    #       filter_files(file_path, range_param)
    #   else:
    #       move_files(file_path)

    # print('Tudo arrumado')


if __name__ == "__main__":
    main()





  