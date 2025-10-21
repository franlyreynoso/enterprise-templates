#!/bin/bash

# Enterprise Blazor Template - Installation and Usage Scripts
# Bash script for Linux/macOS

GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

echo -e "${GREEN}Enterprise Blazor UI Template - Installation Script${NC}"

# Function to install template
install_template() {
    local template_path=${1:-.}
    local force=${2:-false}

    echo -e "${YELLOW}Installing Enterprise Blazor UI Template...${NC}"

    if [ "$force" = true ]; then
        echo -e "${YELLOW}Uninstalling existing template...${NC}"
        dotnet new uninstall "$template_path"
    fi

    dotnet new install "$template_path"

    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅ Template installed successfully!${NC}"
        echo ""
        echo -e "${CYAN}Available presets:${NC}"
        echo -e "${WHITE}  - Full        : Complete enterprise application${NC}"
        echo -e "${WHITE}  - Standard    : Common enterprise features${NC}"
        echo -e "${WHITE}  - Microservice: Microservice-optimized${NC}"
        echo -e "${WHITE}  - Minimal     : Basic Blazor with MudBlazor${NC}"
        echo ""
        echo -e "${CYAN}Example usage:${NC}"
        echo -e "${WHITE}  dotnet new blazor-enterprise -n MyApp --TemplatePreset Full${NC}"
        echo ""
        echo -e "${YELLOW}For detailed usage instructions, see TEMPLATE-USAGE.md${NC}"
    else
        echo -e "${RED}❌ Template installation failed!${NC}"
    fi
}

# Function to create sample projects
create_sample_projects() {
    echo -e "${YELLOW}Creating sample projects...${NC}"

    declare -a samples=(
        "FullEnterpriseApp:Full:Complete enterprise application"
        "StandardBusinessApp:Standard:Standard business application"
        "MicroserviceUI:Microservice:Microservice UI component"
        "MinimalDemo:Minimal:Simple demo application"
    )

    for sample in "${samples[@]}"; do
        IFS=':' read -r name preset description <<< "$sample"
        echo -e "${CYAN}Creating $name ($description)...${NC}"
        dotnet new blazor-enterprise -n "$name" --TemplatePreset "$preset" --force
    done

    echo -e "${GREEN}✅ Sample projects created successfully!${NC}"
}

# Function to test template variations
test_template_variations() {
    echo -e "${YELLOW}Testing template variations...${NC}"

    declare -a test_cases=(
        "AuthOnly:Custom:--IncludeAuth true --IncludeHttpResilience false --IncludeObservability false"
        "NoTesting:Custom:--IncludeAuth true --IncludeHttpResilience true --IncludeTesting false"
        "ObservabilityFocus:Custom:--IncludeObservability true --IncludeAuth false --IncludeHttpResilience true"
    )

    for test in "${test_cases[@]}"; do
        IFS=':' read -r name preset features <<< "$test"
        echo -e "${CYAN}Testing: $name${NC}"

        if dotnet new blazor-enterprise -n "Test$name" --TemplatePreset "$preset" $features --dry-run > /dev/null 2>&1; then
            echo -e "${GREEN}✅ $name configuration valid${NC}"
        else
            echo -e "${RED}❌ $name configuration failed${NC}"
        fi
    done
}

# Function to show menu
show_menu() {
    clear
    echo -e "${GREEN}============================================${NC}"
    echo -e "${GREEN}Enterprise Blazor UI Template Manager${NC}"
    echo -e "${GREEN}============================================${NC}"
    echo ""
    echo -e "${CYAN}1. Install Template${NC}"
    echo -e "${CYAN}2. Reinstall Template (Force)${NC}"
    echo -e "${CYAN}3. Create Sample Projects${NC}"
    echo -e "${CYAN}4. Test Template Variations${NC}"
    echo -e "${CYAN}5. Show Template Help${NC}"
    echo -e "${CYAN}6. List Installed Templates${NC}"
    echo -e "${CYAN}7. Uninstall Template${NC}"
    echo -e "${YELLOW}0. Exit${NC}"
    echo ""
}

# Main script loop
while true; do
    show_menu
    read -p "Select an option: " choice

    case $choice in
        1)
            install_template
            read -p "Press Enter to continue..."
            ;;
        2)
            install_template "." true
            read -p "Press Enter to continue..."
            ;;
        3)
            create_sample_projects
            read -p "Press Enter to continue..."
            ;;
        4)
            test_template_variations
            read -p "Press Enter to continue..."
            ;;
        5)
            dotnet new blazor-enterprise --help
            read -p "Press Enter to continue..."
            ;;
        6)
            dotnet new list
            read -p "Press Enter to continue..."
            ;;
        7)
            echo -e "${YELLOW}Uninstalling template...${NC}"
            dotnet new uninstall "."
            read -p "Press Enter to continue..."
            ;;
        0)
            echo -e "${GREEN}Goodbye!${NC}"
            break
            ;;
        *)
            echo -e "${RED}Invalid option. Please try again.${NC}"
            sleep 2
            ;;
    esac
done