@outerApi
Feature: ConfirmIdentityEnforced
	Force a user to confirm their identity if they try to access an apprentices
	portal page but have not confirmed their identity

Scenario: Redirect to Confirm Your Identity from index
	Given the apprentice has logged in but not created their account
	When the user attempts to land on Apprenticeships index page
	Then redirect the user to the Confirm ID page

Scenario Outline: Redirect to Confirm Your Identity from subpages
	Given the apprentice has logged in but not created their account
	When the user attempts to land on personalised page <page>
	Then redirect the user to the Confirm ID page

	Examples: 
	| page                        |
	| ConfirmYourEmployer         |
	| ConfirmYourTrainingProvider |
	| YourApprenticeshipDetails   |

Scenario: Redirect to home from root when there is no registration code
	Given the apprentice has logged in but not created their account
	When the user attempts to land on root index page
	Then redirect the user to the home page

Scenario: Redirect to NotMatched notification from root index when there is no apprenticeship
	Given the apprentice has logged in but not matched their account
	When the user attempts to land on root index page
	Then redirect the user to the home page with a NotMatched banner

Scenario: Redirect to overview from root when already matched
	Given the apprentice has logged in and matched their account
	When the user attempts to land on root index page
	Then redirect the user to the overview page
