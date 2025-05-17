import json
from pathlib import Path

HEX_STRING_MIN_DIGITS = 4

# MAIN

def main():

    # Open file
    f = open(Path(__file__).parent / ".\\puppets.txt", "r", encoding='utf-8')

    # Open default ids
    jsonFile = open(Path(__file__).parent / ".\\puppets_default_ids.json", "r", encoding='utf-8')
    puppetsIds = json.load(jsonFile)

    # Create output string
    outputStr = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n\n<puppets>\n"

    puppetNumber = 0
    styleId = 0
    lastPuppetName = ""

    # Cycle each line
    line = f.readline()
    while len(line) > 0:
        
        # New puppet style entry
        if line.find("	") != 0 and line.find("\n") != 0:

            # Get puppet parent information
            lineSplit = line.split(" ")
            styleName = lineSplit[0]
            puppetName = lineSplit[1]
            capsPuppetName = puppetName.upper().replace(" ", "_").replace("'","").replace("`","").replace("´","")
            typeStr = lineSplit[2]
            typeStr = typeStr.replace("(", "").replace(")", "")
            typeSplit = typeStr.split("/")
            type1 = typeSplit[0]
            type2 = ""
            if len(typeSplit) > 1: type2 = typeSplit[1]
            cost = lineSplit[3]

            if lastPuppetName != capsPuppetName:

                if puppetNumber > 0: outputStr += "\t\t</styles>\n\t</puppet>\n\n"

                puppetNumber += 1
                styleId = 0
                lastPuppetName = capsPuppetName

                internalId = 0
                puppetdexId = 0

                # Ask for ids
                print("[" + capsPuppetName + "]")
                print("   Internal id for " + capsPuppetName + ":", end="\n    > ")
                if capsPuppetName in puppetsIds:
                    internalId = puppetsIds[capsPuppetName]["id"]
                    print(str(internalId))
                else:
                    strInternalId = input()
                    if len(strInternalId) > 0: internalId = int(strInternalId)
                print("   Puppetdex id for " + capsPuppetName + ":", end="\n    > ")
                if capsPuppetName in puppetsIds:
                    puppetdexId = puppetsIds[capsPuppetName]["dex"]
                    print(str(puppetdexId))
                else:
                    strpuppetdexId = input()
                    if len(strpuppetdexId) > 0: puppetdexId = int(strpuppetdexId)

                # Transform id to hex
                hexInternalId = str(hex(internalId))
                while len(hexInternalId) < HEX_STRING_MIN_DIGITS + 2:
                    hexInternalId = "0x0" + hexInternalId[2:]

                # Write puppet parent information
                outputStr += "\n\t<!-- " + capsPuppetName + " -->\n\n"
                outputStr += "\t<puppet name=\"" + capsPuppetName + "\" id=\"" + hexInternalId + "\">\n"
                outputStr += "\t\t<name>" + puppetName + "</name>\n"
                outputStr += "\t\t<internalId>" + hexInternalId + "</internalId>\n"
                outputStr += "\t\t<dexId>" + str(puppetdexId) + "</dexId>\n"
                outputStr += "\t\t<cost>" + cost + "</cost>\n"
                outputStr += "\t\t<styles>\n\n"

            # Write puppet style information
            outputStr += writeStyles(f, styleName, styleId, type1, type2)
            styleId += 1

        line = f.readline()

    f.close()

    outputStr += "\t\t</styles>\n\t</puppet>\n\n"

    print("Read " + str(puppetNumber) + " puppets in total")

    # Create output file
    outputStr += "\n</puppets>"
    with open(Path(__file__).parent / ".\\Puppets.xml", "w", encoding='utf-8') as f:
        f.write(outputStr)

# WRITE STYLES

def writeStyles(f, styleName, styleId, type1, type2):

    outputStr = ""

    # HP
    line = f.readline()
    hpStat = int(line.replace("	HP: ",""))
    # Fo.Atk
    line = f.readline()
    foAtkStat = int(line.replace("	Fo.Atk: ",""))
    # Fo.Def
    line = f.readline()
    foDefStat = int(line.replace("	Fo.Def: ",""))
    # Sp.Atk
    line = f.readline()
    spAtkStat = int(line.replace("	Sp.Atk: ",""))
    # Sp.Def
    line = f.readline()
    spDefStat = int(line.replace("	Sp.Def: ",""))
    # Speed
    line = f.readline()
    speedStat = int(line.replace("	Speed: ",""))

    f.readline() # BST
    f.readline() # Empty
    f.readline() # Abilities:

    ability1 = ""
    ability2 = ""

    #Ability 1
    line = f.readline()
    ability1 = line.strip().upper()
    ability1 = ability1.replace(" ", "_").replace("'","").replace("`","").replace("´","")
    #Ability 2
    if len(ability1) > 0:
        line = f.readline()
        ability2 = line.strip().upper()
        ability2 = ability2.replace(" ", "_").replace("'","").replace("`","").replace("´","")


    # Write puppet style information
    outputStr += "\t\t\t<style name=\"" + styleName.upper() + "\" id=\"" + str(styleId) + "\">\n"
    outputStr += "\t\t\t\t<type1>" + type1.upper() + "</type1>\n"
    outputStr += "\t\t\t\t<type2>" + type2.upper() + "</type2>\n"
    outputStr += "\t\t\t\t<ability1>" + ability1 + "</ability1>\n"
    outputStr += "\t\t\t\t<ability2>" + ability2 + "</ability2>\n"
    outputStr += "\t\t\t\t<baseStats>\n"
    outputStr += "\t\t\t\t\t<HP>" + str(hpStat) + "</HP>\n"
    outputStr += "\t\t\t\t\t<FAtk>" + str(foAtkStat) + "</FAtk>\n"
    outputStr += "\t\t\t\t\t<FDef>" + str(foDefStat) + "</FDef>\n"
    outputStr += "\t\t\t\t\t<SAtk>" + str(spAtkStat) + "</SAtk>\n"
    outputStr += "\t\t\t\t\t<SDef>" + str(spDefStat) + "</SDef>\n"
    outputStr += "\t\t\t\t\t<Spd>" + str(speedStat) + "</Spd>\n"
    outputStr += "\t\t\t\t</baseStats>\n"
    outputStr += "\t\t\t</style>\n\n"

    return outputStr

# END

main()
