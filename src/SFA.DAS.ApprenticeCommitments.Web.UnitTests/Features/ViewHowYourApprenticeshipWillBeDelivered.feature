@outerApi
Feature: ViewHowYourApprenticeshipWillBeDelivered
	As an apprentice who wants to view how my apprenticeship will be delivered from My apprenticeship page
	So I can review what I previously agreed to

Scenario: The apprentice is authenticated and should see the How your apprenticeship will be delivered page
	Given the apprentice has logged in
	And the apprentice has verified they have read the page
	When accessing the How your apprenticeship will be delivered page from the my apprenticeship page
	Then the response status code should be OK
	And the back link is pointing to the my apprenticeship page
