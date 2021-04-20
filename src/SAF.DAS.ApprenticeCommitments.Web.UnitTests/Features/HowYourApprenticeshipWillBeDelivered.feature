@outerApi
Feature: HowYourApprenticeshipWillBeDelivered
	As an apprentice who wants to view how my apprenticeship will be delivered
	So I can confirm I have read the page

Scenario: The apprentice is authenticated and should see the How your apprenticeship will be delivered page
	Given the apprentice has logged in
	And the apprentice has not verified they have read the page
	When accessing the How your apprenticeship will be delivered page
	Then the response status code should be OK
	And the back link is pointing to the confirm page

Scenario: The apprentice is authenticated and confirms they have read and understand the page
	Given the apprentice has logged in
	And the apprentice has not verified they have read the page
	And the apprentice confirms they understand what they have read
	When submitting the HowYourApprenticeshipWillBeDelivered page
	Then the apprenticeship is updated to show the a 'true' confirmation
	And the user should be redirected back to the overview page

Scenario: The apprentice is authenticated and states they have read and do not understand the page
	Given the apprentice has logged in
	And the apprentice has not verified they have read the page
	And the apprentice states they do not understand what they have read
	When submitting the HowYourApprenticeshipWillBeDelivered page
	Then the apprenticeship is updated to show the a 'false' confirmation
	And the user should be redirected to the cannot confirm apprenticeship page

Scenario: The apprentice is authenticated and presses the Confirm actions without select yes or no
	Given the apprentice has logged in
	And the apprentice has not verified they have read the page
	And the apprentice doesn't select an option
	When submitting the HowYourApprenticeshipWillBeDelivered page
	Then the response status code should be Ok
	And the model should contain an error message
	And the back link is pointing to the confirm page

Scenario: The apprentice is authenticated and has previously negatively confirmed
	Given the apprentice has logged in
	And the apprentice has negatively confirmed they have read the page
	When accessing the How your apprenticeship will be delivered page
	Then the response status code should be OK
	And the user should see the confirmation options
