import { del, get, set } from "idb-keyval"
import { createJSONStorage, persist, type StateStorage } from "zustand/middleware"
import { create } from "zustand";

import { createAccountSlice } from "./slices/userSlice";
import { createLessonSlice } from "./slices/lessonSlice";
import { createCourseSlice } from "./slices/courseSlice";

export type UserStoreState = ReturnType<typeof createAccountSlice>
export type CourseStoreState = ReturnType<typeof createCourseSlice>
export type LessonStoreState = ReturnType<typeof createLessonSlice>

export const useUserStore = create<UserStoreState>()(
  persist(
    (...state) => ({
      ...createAccountSlice(...state)
    }),
    {
      name: 'user-storage',
      storage: createJSONStorage(() => localStorage),
      partialize: state => ({user: state.user})
    }
  )
)
export const useCourseStore = create<CourseStoreState>()(
  persist(
    (...state) => ({
      ...createCourseSlice(...state)
    }),
    {
      name: 'course-storage',
      storage: createJSONStorage(() => sessionStorage),
      partialize: state => ({
        courses: state.courses,
        students: state.students
      })
    }
  )
)
export const useLessonStore = create<LessonStoreState>()(
  persist(
    (...state) => ({
      ...createLessonSlice(...state)
    }),
    {
      name: 'lesson-storage',
      storage: createJSONStorage(() => sessionStorage),
      partialize: state => ({
        lesson: state.lessons,
        course: state.course
      })
    }
  )
)

export const storage: StateStorage = {
  getItem: async (name: string): Promise<string | null> => {
    console.log(name, 'has been retrieved')
    return (await get(name)) || null
  },
  setItem: async (name: string, value: string): Promise<void> => {
    console.log(name, 'with value', value, 'has been saved')
    await set(name, value)
  },
  removeItem: async (name: string): Promise<void> => {
    console.log(name, 'has been deleted')
    await del(name)
  },
}