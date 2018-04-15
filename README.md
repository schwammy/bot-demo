# Introduction 
This is the code sample from my presentation: Creating Awesome Chat Bots with the Bot Framework and C#.
It is the code base that will become the chatbot for the conference TechBash [www.techbash.com](https://www.techbash.com)

Unfortunately, it is not as simple as getting this code, compiling, and running it. The code relies on Azure, LUIS, and the QnA Maker. Instructions below should help you get started. 

# Getting Started
Before using this code you need to get set up
1.	Follow the instructions in the Prerequisites section here: https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-quickstart
2.	Install the emulator. The link is on the same page as above in the section: "Test your bot"
3.  You will also need:
    -  An Azure account: [portal.azure.com](https://portal.azure.com)
    -  A LUIS account: [www.luis.ai](https://www.luis.ai/)
    -  A QnA Maker account: [qnamaker.ai](https://qnamaker.ai/)


# Resources Setup
1.  After you create a LUIS account, create a LUIS app. You can leave it blank if you want. TechBashBot.sln contains a file LuisModel.json that can be imported to get started quickly.
2.  After you create a QnA Maker account, create a QnA service. You can import the questionsn and answers from TechBashBot.sln using the QnAMaker.tsv file.
3.  After you create your Azure Account, create a Web App Bot. Just add a resource and search "bot", then choose Web App Bot

# App Configuration
Once all of your resources are set up, you need to configure the code:
1.  Update the web.config file with the MicrosoftAppId and MicrosoftAppPassword for your new Web App Bot
2.  Update LuisDialog.cs by setting the new LUIS model id and subscription key
3.  Update QnADialog.cs by setting the QnA Service subscription key and knowledgebase id

