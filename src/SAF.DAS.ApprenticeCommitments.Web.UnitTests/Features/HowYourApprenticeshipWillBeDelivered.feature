Feature: HowYourApprenticeshipWillBeDelivered
	As an apprentice who wants to view how my apprenticeship will be delivered
	So I can confirm I have read the page

Scenario: The apprentice is authenticated and should see the How your apprenticeship will be delivered page
	Given the apprentice has logged in
	And the apprentice has not verified they have read the page
	When accessing the How your apprenticeship will be delivered page page
	Then the response status code should be OK
	And the back link is pointing to the My Apprenticships page



