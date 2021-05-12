it('loads examples', () => {
  cy.visit('/');
  cy.contains('podcast-list works!');
});
