namespace SharedImp.Resources

type TextMessages =
    static member UnexpectedException = "❌ Произошла непредвиденная ошибка. Попробуйте позже."
    static member ApiProblem = "❌ Не удалось получить ответ сервера МИТСО. Попробуйте позже."
    static member UserServiceError = "❌ Не удалось получить/обновить информацию от текущем пользователе."
    static member NoFacultiesContent = "⚠️ Не удалось найти факультеты. Попробуйте позже."
    static member NoFormsContent = "⚠️ Не удалось найти формы обучения. Попробуйте позже."
    static member NoCoursesContent = "⚠️ Не удалось найти курсы. Попробуйте позже."
    static member NoGroupsContent = "⚠️ Не удалось найти группы. Попробуйте позже."
    static member NoScheduleContent = "⚠️ Не удалось найти расписание. Попробуйте обновить клавиатуру."
    static member NoTeacherScheduleContent = "⚠️ Не удалось найти расписание преподавателя."
    static member CallbackPayloadOutdated = "⚠️ Не удалось распознать клавиатуру. Пожалуйста, перезапустите диалог через команду /start."
    static member StartMessage = "ℹ️ Используйте меню для поиска расписания группы."
    static member ActualSchedule = "ℹ️ Измененеий в расписании на текущий момент не обнаружено."
    static member SettingsMessage = "ℹ️ Вы открыли меню настроек."
    static member SetFaculty = "🔽 Выберите факультет."
    static member SetForm = "🔽 Выберите форму обучения."
    static member SetCourse = "🔽 Выберите курс."
    static member SetGroup = "🔽 Выберите группу."
    static member RefreshSchedule = "🔽 Используйте эту клавиатуру для поиска расписания."
    static member UseStaticKeyboard = "ℹ️ Используйте статическую клавиатуру для вызова меню."
    static member ScheduleRefreshed = "ℹ️ Клавиатура обновлена."
    static member UseScheduleKeyboard = "🔽 Используйте эту клавиатуру для поиска расписания."
    static member SearchTeacherSchedule = 
        """
        ℹ️ Используйте команду /teacher Фамилия (символ слеша тоже пишем!) для поиска расписания преподавателя. Примеры:
1️⃣ /teacher Фалько
2️⃣ /teacher Лукашевич А. В.
        """