import json
from pathlib import Path

HEX_STRING_MIN_DIGITS = 4

# MAIN

def main():

    # Open skill descriptions file
    skillDescriptions = open(Path(__file__).parent / ".\\SkillData.csv", "r", encoding='ansi')

    # Open skill data json
    jsonFile = open(Path(__file__).parent / ".\\SkillData.json", "r", encoding='utf-8')
    skillData = json.load(jsonFile)["skills"]

    # Create output string
    outputStr = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\n<skills>\n"

    skillNumber = 0

    # Cycle each line
    line = skillDescriptions.readline()
    while len(line) > 0:

        # Get skill description information
        lineSplit = line.split(",")
        skillId = int(lineSplit[0])
        skillName = lineSplit[1]
        capsSkillName = skillName.upper().replace(" ", "_").replace("'","").replace("`","").replace("´","")
        skillDescription = lineSplit[3]
        if len(lineSplit) > 3:
            for currentLineSplit in lineSplit[4:]:
                skillDescription += ", " + currentLineSplit
        skillDescription = skillDescription.strip()

        print("[" + capsSkillName + "]")

        # Get skill data information
        currentSkillData = skillData[skillNumber]
        skillType = currentSkillData["element"].upper()
        skillCategory = currentSkillData["type"].upper()
        skillSp = currentSkillData["sp"]
        skillAccuracy = currentSkillData["accuracy"]
        skillPower = currentSkillData["power"]
        skillPriority = currentSkillData["priority"]

        # Transform id to hex
        hexId = str(hex(skillId))
        while len(hexId) < HEX_STRING_MIN_DIGITS + 2:
            hexId = "0x0" + hexId[2:]

        # Write skill information
        outputStr += "\n\t<!-- " + capsSkillName + " -->\n\n"
        outputStr += "\t<skill name=\"" + capsSkillName + "\" id=\"" + hexId + "\">\n"
        outputStr += "\t\t<name>" + skillName + "</name>\n"
        outputStr += "\t\t<internalId>" + hexId + "</internalId>\n"
        outputStr += "\t\t<description>" + skillDescription + "</description>\n"
        outputStr += "\t\t<type>" + skillType + "</type>\n"
        outputStr += "\t\t<category>" + skillCategory + "</category>\n"
        outputStr += "\t\t<sp>" + skillSp + "</sp>\n"
        outputStr += "\t\t<accuracy>" + skillAccuracy + "</accuracy>\n"
        outputStr += "\t\t<power>" + skillPower + "</power>\n"
        outputStr += "\t\t<priority>" + skillPriority + "</priority>\n"
        outputStr += "\t</skill>\n\n"

        line = skillDescriptions.readline()
        skillNumber += 1


    skillDescriptions.close()
    jsonFile.close()

    print("Read " + str(skillNumber) + " skills in total")

    # Create output file
    outputStr += "\n</skills>"
    with open(Path(__file__).parent / ".\\Skills.xml", "w", encoding='utf-8') as f:
        f.write(outputStr)

# END

main()