@outerApi
Feature: CreateAccountEnforced
	Force a user to create their account if they try to access an apprentices
	portal page but have not created an account

Scenario: Redirect to Account from index
	Given the apprentice has logged in but not created their account
	When the user attempts to land on Apprenticeships index page
	Then redirect the user to the Account page

Scenario Outline: Redirect to Account from subpages
	Given the apprentice has logged in but not created their account
	When the user attempts to land on personalised page <page>
	Then redirect the user to the Account page

	Examples: 
	| page                        |
	| ConfirmYourEmployer         |
	| ConfirmYourTrainingProvider |
	| YourApprenticeshipDetails   |

Scenario: Redirect to account from apprenticeships when there is registration code
	Given the apprentice has logged in but not created their account
	When the user attempts to land on the Register page with a registration code
	Then redirect the user to the Account page
	And store the registration code in a cookie

Scenario: Redirect to home from root when there is no registration code
	Given the apprentice has logged in but not created their account
	When the user attempts to land on root index page
	Then redirect the user to the Account page

Scenario: Redirect to NotMatched notification from root index when there is no apprenticeship
	Given the apprentice has logged in but not matched their account
	When the user attempts to land on root index page
	Then redirect the user to the home page with a NotMatched banner

Scenario: Redirect to overview from root when already matched
	Given the apprentice has logged in and matched their account
	When the user attempts to land on root index page
	Then redirect the user to the overview page

Scenario: Redirect to Terms Of Use from index
	Given the apprentice has logged in but not accepted the terms of use
	When the user attempts to land on Apprenticeships index page
	Then redirect the user to the TermsOfUse page

Scenario Outline: Redirect to Terms Of Use from subpages
	Given the apprentice has logged in but not accepted the terms of use
	When the user attempts to land on personalised page <page>
	Then redirect the user to the TermsOfUse page

	Examples: 
	| page                        |
	| ConfirmYourEmployer         |
	| ConfirmYourTrainingProvider |
	| YourApprenticeshipDetails   |
