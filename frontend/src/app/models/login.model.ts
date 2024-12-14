export interface Login {
    IdToken: string,
    LocalId: string,
    Email: string
}

export interface LoginRequest
{
    Email: string;
    Password: string;
}