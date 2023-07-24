namespace ChattingApp.Users;

public static class UserMapper
{
    public static User RegisterUserDtoToUser(this RegisterUserDto registerUserDto)
    {
        return new User
        {
            UserName = registerUserDto.Email,
            Email = registerUserDto.Email,
            FirstName = registerUserDto.FirstName,
            LastName = registerUserDto.LastName,
        };
    }
    
    public static GetUserDto UserToGetUserDto(this User user)
    {
        return new GetUserDto
        {
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }
}