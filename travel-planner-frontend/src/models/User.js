export class User {
  constructor(data = {}) {
    // Backend AuthResponseDto vraca: userId, firstName, lastName, email, role, token
    // UserDto (admin lista) vraca: id, firstName, lastName, email, role, createdAt
    this.id = data.userId || data.id || 0;
    this.firstName = data.firstName || '';
    this.lastName = data.lastName || '';
    this.name = `${data.firstName || ''} ${data.lastName || ''}`.trim();
    this.email = data.email || '';
    this.role = data.role || 'user';
    this.createdAt = data.createdAt || '';
  }
}
